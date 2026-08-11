import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CoreDomainApiService {
  private readonly http = inject(HttpClient);

  get<T>(path: string, search?: string) {
    const params = search ? new HttpParams().set('search', search) : undefined;
    return this.http.get<T>(path, { params });
  }

  getWithParams<T>(path: string, parameters: Record<string, string>) {
    return this.http.get<T>(path, {
      params: new HttpParams({ fromObject: parameters }),
    });
  }

  post<TResponse>(path: string, body: object) {
    return this.http.post<TResponse>(path, body);
  }

  put<TResponse>(path: string, body: object) {
    return this.http.put<TResponse>(path, body);
  }
}
