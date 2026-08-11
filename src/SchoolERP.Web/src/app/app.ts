import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { HealthService } from './core/health.service';

type HealthState = 'checking' | 'healthy' | 'unavailable';

@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private readonly healthService = inject(HealthService);

  protected readonly healthState = signal<HealthState>('checking');

  constructor() {
    this.healthService.checkLiveness().subscribe({
      next: () => this.healthState.set('healthy'),
      error: () => this.healthState.set('unavailable'),
    });
  }
}
