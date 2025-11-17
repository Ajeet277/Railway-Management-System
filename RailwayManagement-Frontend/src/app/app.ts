import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/shared/header/header';
import { TrainService } from './services/train';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent implements OnInit {
  title = 'Railway Management System';

  constructor(private trainService: TrainService) {}

  ngOnInit() {
    // Test API connection
    this.trainService.getAllTrains().subscribe({
      next: (trains) => console.log('✅ Backend Connected:', trains),
      error: (error) => console.error('❌ Backend Connection Failed:', error)
    });
  }
}
