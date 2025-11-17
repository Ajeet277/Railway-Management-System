import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService, Train, AddTrainRequest, UpdateTrainRequest } from '../../services/admin';

@Component({
  selector: 'app-admin-trains',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-trains.html',
  styleUrl: './admin-trains.css'
})
export class AdminTrainsComponent implements OnInit {
  trains: Train[] = [];
  trainForm: FormGroup;
  editingTrain: Train | null = null;
  showForm = false;
  loading = false;
  
  // Toast notification
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' = 'success';
  
  // Delete modal
  showDeleteModal = false;
  selectedTrain: Train | null = null;
  deleting = false;

  constructor(
    private adminService: AdminService,
    private formBuilder: FormBuilder
  ) {
    this.trainForm = this.formBuilder.group({
      trainNumber: ['', [Validators.required, Validators.pattern(/^[0-9]+$/)]],
      trainName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]+$/), Validators.minLength(3)]],
      source: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]+$/), Validators.minLength(2)]],
      destination: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]+$/), Validators.minLength(2)]],
      departureTime: ['', Validators.required],
      arrivalTime: ['', Validators.required],
      totalSeats: ['', [Validators.required, Validators.min(1), Validators.max(1000)]],
      fare: ['', [Validators.required, Validators.min(1), Validators.max(50000)]],
      class: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadTrains();
  }

  loadTrains() {
    this.loading = true;
    this.adminService.getAllTrains().subscribe({
      next: (trains) => {
        this.trains = trains;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading trains:', error);
        this.loading = false;
      }
    });
  }

  showAddForm() {
    this.editingTrain = null;
    this.trainForm.reset();
    this.showForm = true;
  }

  editTrain(train: Train) {
    this.editingTrain = train;
    this.trainForm.patchValue({
      trainNumber: train.trainNumber,
      trainName: train.trainName,
      source: train.source,
      destination: train.destination,
      departureTime: this.formatTimeForInput(train.departureTime),
      arrivalTime: this.formatTimeForInput(train.arrivalTime),
      totalSeats: train.totalSeats,
      fare: train.fare,
      class: train.class
    });
    this.showForm = true;
  }

  private formatTimeForInput(timeString: string): string {
    // Convert "HH:MM:SS" or "HH:MM" to "HH:MM" format for time input
    if (!timeString) return '';
    const parts = timeString.split(':');
    return `${parts[0]}:${parts[1]}`;
  }

  onSubmit() {
    if (this.trainForm.invalid) return;

    if (this.editingTrain) {
      const formValue = this.trainForm.value;
      const updateData: UpdateTrainRequest = {};
      
      // Only include fields that have values
      if (formValue.trainName) updateData.trainName = formValue.trainName;
      if (formValue.source) updateData.source = formValue.source;
      if (formValue.destination) updateData.destination = formValue.destination;
      if (formValue.departureTime) updateData.departureTime = formValue.departureTime;
      if (formValue.arrivalTime) updateData.arrivalTime = formValue.arrivalTime;
      if (formValue.totalSeats) updateData.totalSeats = formValue.totalSeats;
      if (formValue.fare) updateData.fare = formValue.fare;
      if (formValue.class) updateData.class = formValue.class;
      
      console.log('Sending update data:', updateData);
      
      this.adminService.updateTrain(this.editingTrain.id, updateData).subscribe({
        next: () => {
          this.loadTrains();
          this.cancelForm();
          this.showToastMessage('Train updated successfully! ✅', 'success');
        },
        error: (error) => {
          console.error('Update error:', error);
          this.showToastMessage('Failed to update train ❌', 'error');
        }
      });
    } else {
      const trainData: AddTrainRequest = this.trainForm.value;
      this.adminService.addTrain(trainData).subscribe({
        next: () => {
          this.loadTrains();
          this.cancelForm();
          this.showToastMessage('Train added successfully! 🚂', 'success');
        },
        error: (error) => this.showToastMessage('Failed to add train ❌', 'error')
      });
    }
  }

  openDeleteModal(train: Train) {
    this.selectedTrain = train;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.selectedTrain = null;
    this.deleting = false;
  }

  confirmDelete() {
    if (!this.selectedTrain) return;

    this.deleting = true;
    this.adminService.deleteTrain(this.selectedTrain.id).subscribe({
      next: () => {
        this.loadTrains();
        this.closeDeleteModal();
        this.showToastMessage('Train deleted successfully! 🗑️', 'success');
      },
      error: (error) => {
        this.showToastMessage('Failed to delete train ❌', 'error');
        this.deleting = false;
      }
    });
  }

  cancelForm() {
    this.showForm = false;
    this.editingTrain = null;
    this.trainForm.reset();
  }

  showToastMessage(message: string, type: 'success' | 'error') {
    this.toastMessage = message;
    this.toastType = type;
    this.showToast = true;
    
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }
}