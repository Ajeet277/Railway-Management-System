import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../services/payment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-payment',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './payment.html',
  styleUrl: './payment.css',
})
export class PaymentComponent implements OnInit {
  paymentForm: FormGroup;
  reservationId: number = 0;
  selectedMethod: string = 'CreditCard';
  loading = false;
  error = '';
  paymentSuccess = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService
  ) {
    this.paymentForm = this.formBuilder.group({
      paymentMethod: ['CreditCard', Validators.required],
      cardNumber: ['', Validators.required],
      cardHolderName: ['', Validators.required],
      expiryMonth: ['', Validators.required],
      expiryYear: ['', Validators.required],
      cvv: ['', Validators.required],
      upiId: [''],
      bankCode: ['']
    });
  }

  ngOnInit() {
    // Get reservationId from route parameter
    this.reservationId = Number(this.route.snapshot.paramMap.get('reservationId'));
    console.log('Payment component reservationId:', this.reservationId);
    
    this.onPaymentMethodChange();
  }

  onPaymentMethodChange() {
    this.selectedMethod = this.paymentForm.get('paymentMethod')?.value;
    this.updateValidators();
  }

  updateValidators() {
    // Clear all validators first
    Object.keys(this.paymentForm.controls).forEach(key => {
      if (key !== 'paymentMethod') {
        this.paymentForm.get(key)?.clearValidators();
      }
    });

    // Add validators based on payment method
    if (this.selectedMethod === 'CreditCard' || this.selectedMethod === 'DebitCard') {
      this.paymentForm.get('cardNumber')?.setValidators([Validators.required]);
      this.paymentForm.get('cardHolderName')?.setValidators([Validators.required]);
      this.paymentForm.get('expiryMonth')?.setValidators([Validators.required]);
      this.paymentForm.get('expiryYear')?.setValidators([Validators.required]);
      this.paymentForm.get('cvv')?.setValidators([Validators.required]);
    } else if (this.selectedMethod === 'UPI') {
      this.paymentForm.get('upiId')?.setValidators([Validators.required]);
    } else if (this.selectedMethod === 'NetBanking') {
      this.paymentForm.get('bankCode')?.setValidators([Validators.required]);
    }

    // Update form validation
    Object.keys(this.paymentForm.controls).forEach(key => {
      this.paymentForm.get(key)?.updateValueAndValidity();
    });
  }

  onSubmit() {
    if (this.paymentForm.invalid) return;
    
    if (!this.reservationId || this.reservationId === 0) {
      this.error = 'Invalid reservation ID. Cannot process payment.';
      return;
    }

    this.loading = true;
    this.error = '';

    const formValue = this.paymentForm.value;
    const paymentData: any = {
      reservationId: this.reservationId,
      paymentMethod: formValue.paymentMethod
    };

    // Only add relevant fields based on payment method
    if (formValue.paymentMethod === 'CreditCard' || formValue.paymentMethod === 'DebitCard') {
      paymentData.cardNumber = formValue.cardNumber;
      paymentData.cardHolderName = formValue.cardHolderName;
      paymentData.expiryDate = `${formValue.expiryMonth}/${formValue.expiryYear}`;
      paymentData.cvv = formValue.cvv;
    } else if (formValue.paymentMethod === 'UPI') {
      paymentData.upiId = formValue.upiId;
    } else if (formValue.paymentMethod === 'NetBanking') {
      paymentData.bankCode = formValue.bankCode;
    }

    console.log('Payment data being sent:', paymentData);
    
    this.paymentService.processPayment(paymentData).subscribe({
      next: (response) => {
        this.loading = false;
        console.log('Payment response:', response);
        // Check for success status (case insensitive)
        const isSuccess = response?.status?.toLowerCase() === 'success';
        
        if (isSuccess) {
          this.paymentSuccess = true;
          // Clear pending payment from session storage
          sessionStorage.removeItem('pendingPayment');
        } else {
          this.error = response?.message || 'Payment failed';
        }
      },
      error: (error) => {
        this.loading = false;
        console.error('Payment error:', error);
        this.error = error.error?.message || 'Payment processing failed';
      }
    });
  }
  
  goToBookings() {
    this.router.navigate(['/my-bookings']);
  }
  
  goToHome() {
    this.router.navigate(['/train-search']);
  }
}
