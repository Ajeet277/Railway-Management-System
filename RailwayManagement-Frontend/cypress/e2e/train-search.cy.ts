describe('Train Search', () => {
  beforeEach(() => {
    cy.visit('/train-search');
  });

  it('should display search form', () => {
    cy.contains('Discover Your Journey').should('be.visible');
    cy.get('input[placeholder="Source Station"]').should('be.visible');
    cy.get('input[placeholder="Destination Station"]').should('be.visible');
    cy.get('input[type="date"]').should('be.visible');
  });

  it('should search trains by route', () => {
    cy.intercept('GET', '**/api/train/search*', {
      statusCode: 200,
      body: [
        {
          id: 1,
          trainNumber: '12345',
          trainName: 'Test Express',
          source: 'Delhi',
          destination: 'Mumbai',
          fare: 2500,
          availableSeats: 100
        }
      ]
    }).as('searchTrains');

    cy.get('input[placeholder="Source Station"]').type('Delhi');
    cy.get('input[placeholder="Destination Station"]').type('Mumbai');
    cy.get('input[type="date"]').type('2024-12-25');
    cy.contains('Search Trains').click();

    cy.contains('Test Express', { timeout: 10000 }).should('be.visible');
  });

  it('should search trains by number', () => {
    cy.get('button').contains('Search by Number').click();
    
    cy.intercept('GET', '**/api/train/number/*', {
      statusCode: 200,
      body: [
        {
          id: 1,
          trainNumber: '12345',
          trainName: 'Test Express',
          source: 'Delhi',
          destination: 'Mumbai',
          fare: 2500,
          availableSeats: 100
        }
      ]
    }).as('searchByNumber');

    cy.get('input[placeholder="Enter Train Number"]').type('12345');
    cy.get('input[type="date"]').type('2024-12-25');
    cy.contains('Search Train').click();

    cy.contains('Test Express', { timeout: 10000 }).should('be.visible');
  });

  it('should show no results message', () => {
    cy.intercept('GET', '**/api/train/search*', {
      statusCode: 200,
      body: []
    }).as('noResults');

    cy.get('input[placeholder="Source Station"]').type('Delhi');
    cy.get('input[placeholder="Destination Station"]').type('Mumbai');
    cy.get('input[type="date"]').type('2024-12-25');
    cy.contains('Search Trains').click();

    cy.contains('No trains found', { timeout: 10000 }).should('be.visible');
  });
});