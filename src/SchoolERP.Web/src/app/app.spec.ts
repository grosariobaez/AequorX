import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { I18nService } from './core/i18n.service';
import { PeoplePage } from './pages/people.page';

describe('Core domain administration', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeoplePage],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('uses Spanish by default and can switch to English', () => {
    const i18n = TestBed.inject(I18nService);

    expect(i18n.text('people')).toBe('Personas');

    i18n.setLanguage('en');

    expect(i18n.text('people')).toBe('People');
    expect(document.documentElement.lang).toBe('en');
  });

  it('creates and reloads a person through the API workflow', async () => {
    const fixture = TestBed.createComponent(PeoplePage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/people').flush([]);
    await fixture.whenStable();

    const inputs = fixture.nativeElement.querySelectorAll('input') as NodeListOf<HTMLInputElement>;
    inputs[1].value = 'Ana';
    inputs[1].dispatchEvent(new Event('input'));
    inputs[2].value = 'Pérez';
    inputs[2].dispatchEvent(new Event('input'));
    fixture.detectChanges();
    await fixture.whenStable();

    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));

    const createRequest = http.expectOne('/api/people');
    expect(createRequest.request.method).toBe('POST');
    expect(createRequest.request.body).toEqual({
      firstName: 'Ana',
      lastName: 'Pérez',
      email: '',
      phone: '',
    });
    createRequest.flush({
      id: '11111111-1111-1111-1111-111111111111',
      firstName: 'Ana',
      lastName: 'Pérez',
      email: null,
      phone: null,
      isActive: true,
    });

    http.expectOne('/api/people').flush([
      {
        id: '11111111-1111-1111-1111-111111111111',
        firstName: 'Ana',
        lastName: 'Pérez',
        email: null,
        phone: null,
        isActive: true,
      },
    ]);
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Ana Pérez');
  });
});
