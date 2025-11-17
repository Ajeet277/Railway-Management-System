import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TrainService } from '../../services/train';
import { Train } from '../../models/train.model';
import { AuthService } from '../../services/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-train-search',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './train-search.html',
  styleUrl: './train-search.css',
})
export class TrainSearchComponent implements OnInit {
  searchForm: FormGroup;
  trains: Train[] = [];
  loading = false;
  searched = false;
  searchType: 'route' | 'number' = 'route';

  // Date restrictions
  minDate: string = '';
  maxDate: string = '';

  // Station autocomplete - populated from actual train data
  stations: string[] = [];
  
  filteredSourceStations: string[] = [];
  filteredDestinationStations: string[] = [];
  showSourceDropdown = false;
  showDestinationDropdown = false;

  constructor(
    private formBuilder: FormBuilder,
    private trainService: TrainService,
    private router: Router,
    public authService: AuthService
  ) {
    this.setDateLimits();
    this.searchForm = this.formBuilder.group({
      source: [''],
      destination: [''],
      trainNumber: [''],
      date: ['', Validators.required]
    });
    this.updateValidators();
  }

  ngOnInit() {
    this.loadAvailableStations();
  }

  setDateLimits() {
    const today = new Date();
    const twoMonthsLater = new Date();
    twoMonthsLater.setMonth(today.getMonth() + 2);
    
    this.minDate = today.toISOString().split('T')[0];
    this.maxDate = twoMonthsLater.toISOString().split('T')[0];
  }

  setSearchType(type: 'route' | 'number') {
    this.searchType = type;
    this.searched = false;
    this.trains = [];
    this.updateValidators();
  }

  updateValidators() {
    if (this.searchType === 'route') {
      this.searchForm.get('source')?.setValidators([Validators.required]);
      this.searchForm.get('destination')?.setValidators([Validators.required, this.differentStationValidator.bind(this)]);
      this.searchForm.get('trainNumber')?.clearValidators();
    } else {
      this.searchForm.get('trainNumber')?.setValidators([Validators.required]);
      this.searchForm.get('source')?.clearValidators();
      this.searchForm.get('destination')?.clearValidators();
    }
    
    // Update validity for each control
    this.searchForm.get('source')?.updateValueAndValidity();
    this.searchForm.get('destination')?.updateValueAndValidity();
    this.searchForm.get('trainNumber')?.updateValueAndValidity();
  }
  
  differentStationValidator(control: any) {
    const source = this.searchForm?.get('source')?.value;
    const destination = control.value;
    
    if (source && destination && source.toLowerCase() === destination.toLowerCase()) {
      return { sameStation: true };
    }
    return null;
  }

  loadAvailableStations() {
    this.trainService.getAllTrains().subscribe({
      next: (trains) => {
        const stationSet = new Set<string>();
        trains.forEach(train => {
          stationSet.add(train.source);
          stationSet.add(train.destination);
        });
        this.stations = Array.from(stationSet).sort();
      },
      error: (error) => {
        console.error('Error loading stations:', error);
      }
    });
  }

  onSearch() {
    if (this.searchForm.invalid) return;

    this.loading = true;
    
    if (this.searchType === 'route') {
      const searchRequest = {
        source: this.searchForm.get('source')?.value,
        destination: this.searchForm.get('destination')?.value,
        date: this.searchForm.get('date')?.value
      };
      
      this.trainService.searchTrains(searchRequest).subscribe({
        next: (trains) => {
          this.trains = trains;
          this.loading = false;
          this.searched = true;
        },
        error: () => {
          this.loading = false;
          this.searched = true;
        }
      });
    } else {
      const trainNumber = this.searchForm.get('trainNumber')?.value;
      console.log('Searching for train number:', trainNumber);
      
      this.trainService.searchTrainByNumber(trainNumber).subscribe({
        next: (trains) => {
          console.log('Train search results:', trains);
          this.trains = trains;
          this.loading = false;
          this.searched = true;
        },
        error: (error) => {
          console.error('Train search error:', error);
          this.trains = [];
          this.loading = false;
          this.searched = true;
        }
      });
    }
  }

  bookTrain(trainId: number) {
    if (!this.authService.isAuthenticated()) {
      // Store intended booking URL before redirecting to login
      const journeyDate = this.searchForm.get('date')?.value;
      const intendedUrl = `/booking/${trainId}?date=${journeyDate}`;
      sessionStorage.setItem('redirectAfterLogin', intendedUrl);
      this.router.navigate(['/login']);
      return;
    }
    
    const journeyDate = this.searchForm.get('date')?.value;
    this.router.navigate(['/booking', trainId], { 
      queryParams: { date: journeyDate } 
    });
  }

  // Swap source and destination
  swapStations() {
    const source = this.searchForm.get('source')?.value;
    const destination = this.searchForm.get('destination')?.value;
    
    this.searchForm.patchValue({
      source: destination,
      destination: source
    });
  }

  // Handle station input for autocomplete
  onStationInput(field: string, event: any) {
    const value = event.target.value.toLowerCase();
    const filtered = this.stations.filter(station => 
      station.toLowerCase().includes(value)
    ).slice(0, 5); // Show max 5 suggestions
    
    if (field === 'source') {
      this.filteredSourceStations = filtered;
    } else {
      this.filteredDestinationStations = filtered;
    }
  }

  // Show dropdown
  showDropdown(field: string) {
    if (field === 'source') {
      this.showSourceDropdown = true;
      this.filteredSourceStations = this.stations.slice(0, 5);
    } else {
      this.showDestinationDropdown = true;
      this.filteredDestinationStations = this.stations.slice(0, 5);
    }
  }

  // Hide dropdown
  hideDropdown(field: string) {
    setTimeout(() => {
      if (field === 'source') {
        this.showSourceDropdown = false;
      } else {
        this.showDestinationDropdown = false;
      }
    }, 200); // Delay to allow click on dropdown item
  }

  // Select station from dropdown
  selectStation(field: string, station: string) {
    this.searchForm.patchValue({ [field]: station });
    
    // Trigger validation for destination when source changes
    if (field === 'source') {
      this.searchForm.get('destination')?.updateValueAndValidity();
      this.showSourceDropdown = false;
    } else {
      this.showDestinationDropdown = false;
    }
  }
}
