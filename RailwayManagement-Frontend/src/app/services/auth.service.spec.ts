import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth';
import { environment } from '../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    
    // Clear localStorage before each test
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login user successfully', () => {
    const mockLoginRequest = { email: 'test@test.com', password: 'password123' };
    const mockResponse = { 
      token: 'test-token', 
      name: 'Test User', 
      email: 'test@test.com', 
      role: 'User' 
    };

    service.login(mockLoginRequest).subscribe(response => {
      expect(response).toEqual(mockResponse);
      expect(localStorage.getItem('token')).toBe('test-token');
      expect(localStorage.getItem('user')).toBeTruthy();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockLoginRequest);
    req.flush(mockResponse);
  });

  it('should register user successfully', () => {
    const mockRegisterRequest = {
      name: 'Test User',
      email: 'test@test.com',
      phone: '9876543210',
      password: 'password123',
      address: 'Test Address'
    };
    const mockResponse = { message: 'Registration successful' };

    service.register(mockRegisterRequest).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockRegisterRequest);
    req.flush(mockResponse);
  });

  it('should logout user', () => {
    // Set up localStorage with user data
    localStorage.setItem('token', 'test-token');
    localStorage.setItem('user', JSON.stringify({ name: 'Test User' }));

    service.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('user')).toBeNull();
    expect(service.currentUserValue).toBeNull();
  });

  it('should return token', () => {
    localStorage.setItem('token', 'test-token');
    expect(service.getToken()).toBe('test-token');
  });

  it('should check if user is authenticated', () => {
    expect(service.isAuthenticated()).toBeFalse();
    
    localStorage.setItem('token', 'test-token');
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('should check if user is admin', () => {
    const adminUser = { id: 1, name: 'Admin', email: 'admin@test.com', role: 'Admin' };
    service['currentUserSubject'].next(adminUser);
    
    expect(service.isAdmin()).toBeTrue();
  });

  it('should get user from storage', () => {
    const userData = { id: 1, name: 'Test User', email: 'test@test.com', role: 'User' };
    localStorage.setItem('user', JSON.stringify(userData));
    
    const user = service['getUserFromStorage']();
    expect(user).toEqual(userData);
  });
});