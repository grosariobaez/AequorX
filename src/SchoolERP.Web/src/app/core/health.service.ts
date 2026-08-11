import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class HealthService {
  private readonly http = inject(HttpClient);

  checkLiveness(): Observable<string> {
    return this.http.get('/health/live', { responseType: 'text' });
  }
}
