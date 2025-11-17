import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent {
  registerForm: FormGroup;
  loading = false;
  error = '';
  success = '';
  passwordStrength = 'weak';
  showTermsModal = false;
  showPrivacyModal = false;
  passwordRequirements = {
    length: false,
    uppercase: false,
    lowercase: false,
    number: false,
    special: false
  };

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
      phone: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)]],
      address: ['', [Validators.required, Validators.minLength(10)]],
      acceptTerms: [false, Validators.requiredTrue]
    });
  }

  checkPasswordStrength(event: any) {
    const password = event.target.value;
    
    // Check requirements
    this.passwordRequirements = {
      length: password.length >= 8,
      uppercase: /[A-Z]/.test(password),
      lowercase: /[a-z]/.test(password),
      number: /\d/.test(password),
      special: /[@$!%*?&]/.test(password)
    };
    
    // Calculate strength
    const metRequirements = Object.values(this.passwordRequirements).filter(Boolean).length;
    
    if (metRequirements <= 2) {
      this.passwordStrength = 'weak';
    } else if (metRequirements === 3) {
      this.passwordStrength = 'fair';
    } else if (metRequirements === 4) {
      this.passwordStrength = 'good';
    } else {
      this.passwordStrength = 'strong';
    }
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.error = '';
    this.success = '';

    const formData = { ...this.registerForm.value };
    delete formData.acceptTerms; // Remove acceptTerms from API call

    this.authService.register(formData).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Registration successful! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
  
  showTerms(event: Event) {
    event.preventDefault();
    this.showTermsModal = true;
  }
  
  showPrivacy(event: Event) {
    event.preventDefault();
    this.showPrivacyModal = true;
  }
  
  closeModal() {
    this.showTermsModal = false;
    this.showPrivacyModal = false;
  }
}
