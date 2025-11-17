describe('Login Flow', () => {
  beforeEach(() => {
    cy.visit('/login');
  });

  it('should display login form', () => {
    cy.contains('Welcome Back').should('be.visible');
    cy.get('input[type="email"]').should('be.visible');
    cy.get('input[type="password"]').should('be.visible');
    cy.get('button[type="submit"]').should('be.visible');
  });

  it('should validate email field', () => {
    cy.get('input[type="email"]').type('invalid-email');
    cy.get('input[type="email"]').blur();
    cy.get('input[type="email"]').should('have.class', 'error');
  });

  it('should login successfully with valid credentials', () => {
    cy.intercept('POST', '**/auth/login', {
      statusCode: 200,
      body: {
        token: 'test-token',
        name: 'Test User',
        email: 'test@test.com',
        role: 'User'
      }
    }).as('loginRequest');

    cy.get('input[type="email"]').type('test@test.com');
    cy.get('input[type="password"]').type('password123');
    cy.get('button[type="submit"]').click();

    cy.wait('@loginRequest');
    cy.url().should('include', '/dashboard');
  });

  it('should show error for invalid credentials', () => {
    cy.intercept('POST', '**/auth/login', {
      statusCode: 401,
      body: { message: 'Invalid credentials' }
    }).as('loginError');

    cy.get('input[type="email"]').type('invalid@test.com');
    cy.get('input[type="password"]').type('wrongpassword');
    cy.get('button[type="submit"]').click();

    cy.wait('@loginError');
    cy.contains('Invalid email or password').should('be.visible');
  });
});