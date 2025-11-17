import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { TrainSearchComponent } from './train-search';
import { TrainService } from '../../services/train';

describe('TrainSearchComponent', () => {
  let component: TrainSearchComponent;
  let fixture: ComponentFixture<TrainSearchComponent>;
  let trainService: jasmine.SpyObj<TrainService>;
  let router: jasmine.SpyObj<Router>;

  const mockTrains = [
    {
      id: 1,
      trainNumber: '12345',
      trainName: 'Test Express',
      source: 'Delhi',
      destination: 'Mumbai',
      departureTime: '16:30',
      arrivalTime: '08:30',
      fare: 2500,
      class: 'AC',
      availableSeats: 100,
      totalSeats: 200
    }
  ];

  beforeEach(async () => {
    const trainSpy = jasmine.createSpyObj('TrainService', ['searchTrains', 'getAllTrains', 'searchTrainByNumber']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, RouterTestingModule, TrainSearchComponent],
      providers: [
        { provide: TrainService, useValue: trainSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TrainSearchComponent);
    component = fixture.componentInstance;
    trainService = TestBed.inject(TrainService) as jasmine.SpyObj<TrainService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    trainService.getAllTrains.and.returnValue(of(mockTrains));
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with route search type', () => {
    expect(component.searchType).toBe('route');
  });

  it('should load available stations on init', () => {
    component.ngOnInit();
    
    expect(trainService.getAllTrains).toHaveBeenCalled();
    expect(component.stations).toContain('Delhi');
    expect(component.stations).toContain('Mumbai');
  });

  it('should set search type and update validators', () => {
    component.setSearchType('number');
    
    expect(component.searchType).toBe('number');
    expect(component.searched).toBeFalse();
    expect(component.trains).toEqual([]);
  });

  it('should search trains by route', () => {
    trainService.searchTrains.and.returnValue(of(mockTrains));
    
    component.searchForm.patchValue({
      source: 'Delhi',
      destination: 'Mumbai',
      date: '2024-12-25'
    });
    
    component.onSearch();
    
    expect(trainService.searchTrains).toHaveBeenCalled();
    expect(component.trains).toEqual(mockTrains);
    expect(component.searched).toBeTrue();
  });

  it('should search trains by number', () => {
    component.setSearchType('number');
    trainService.searchTrainByNumber.and.returnValue(of(mockTrains));
    
    component.searchForm.patchValue({
      trainNumber: '12345',
      date: '2024-12-25'
    });
    
    component.onSearch();
    
    expect(trainService.searchTrainByNumber).toHaveBeenCalledWith('12345');
    expect(component.trains).toEqual(mockTrains);
  });

  it('should handle search error', () => {
    trainService.searchTrains.and.returnValue(throwError(() => new Error('Search failed')));
    
    component.searchForm.patchValue({
      source: 'Delhi',
      destination: 'Mumbai',
      date: '2024-12-25'
    });
    
    component.onSearch();
    
    expect(component.loading).toBeFalse();
    expect(component.searched).toBeTrue();
  });

  it('should navigate to booking page', () => {
    component.bookTrain(1);
    
    expect(router.navigate).toHaveBeenCalledWith(['/booking', 1]);
  });

  it('should swap stations', () => {
    component.searchForm.patchValue({
      source: 'Delhi',
      destination: 'Mumbai'
    });
    
    component.swapStations();
    
    expect(component.searchForm.get('source')?.value).toBe('Mumbai');
    expect(component.searchForm.get('destination')?.value).toBe('Delhi');
  });

  it('should filter stations on input', () => {
    component.stations = ['Delhi', 'Mumbai', 'Kolkata'];
    const event = { target: { value: 'del' } };
    
    component.onStationInput('source', event);
    
    expect(component.filteredSourceStations).toEqual(['Delhi']);
  });

  it('should select station from dropdown', () => {
    component.selectStation('source', 'Delhi');
    
    expect(component.searchForm.get('source')?.value).toBe('Delhi');
    expect(component.showSourceDropdown).toBeFalse();
  });
});