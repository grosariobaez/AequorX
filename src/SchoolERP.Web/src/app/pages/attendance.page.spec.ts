import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { I18nService } from '../core/i18n.service';
import { AttendancePage } from './attendance.page';

describe('Attendance entry', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AttendancePage],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('loads Present by default, saves an exception, and localizes the workflow', async () => {
    const fixture = TestBed.createComponent(AttendancePage);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    http.expectOne('/api/sections').flush([
      {
        id: 'section-1',
        name: 'A',
        code: 'A',
        capacity: 30,
        academicYearId: 'year-1',
        academicYearName: '2026-2027',
        gradeLevelId: 'grade-1',
        gradeLevelName: 'Primero',
        campusId: 'campus-1',
        campusName: 'Principal',
      },
    ]);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const date = fixture.nativeElement.querySelector('input[type="date"]') as HTMLInputElement;
    date.value = '2026-09-01';
    date.dispatchEvent(new Event('input', { bubbles: true }));
    const section = fixture.nativeElement.querySelector('select') as HTMLSelectElement;
    section.value = 'section-1';
    section.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('form button') as HTMLButtonElement).click();
    const rosterRequest = http.expectOne(
      (request) =>
        request.url === '/api/attendance' &&
        request.params.get('sectionId') === 'section-1' &&
        request.params.get('date') === '2026-09-01',
    );
    rosterRequest.flush(roster('Present'));
    fixture.detectChanges();
    await fixture.whenStable();

    const status = fixture.nativeElement.querySelectorAll('select')[1] as HTMLSelectElement;
    expect(status.value).toBe('Present');
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Presente');

    status.value = 'Absent';
    status.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    const save = fixture.nativeElement.querySelector('.command-bar button') as HTMLButtonElement;
    save.click();

    const update = http.expectOne('/api/attendance/enrollment-1/2026-09-01');
    expect(update.request.method).toBe('PUT');
    expect(update.request.body).toEqual({ status: 'Absent', note: null });
    update.flush(roster('Absent').students[0]);
    http.expectOne((request) => request.url === '/api/attendance').flush(roster('Absent'));

    TestBed.inject(I18nService).setLanguage('en');
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Attendance');
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Absent');
  });
});

function roster(status: 'Present' | 'Absent') {
  return {
    sectionId: 'section-1',
    sectionName: 'A',
    date: '2026-09-01',
    students: [
      {
        enrollmentId: 'enrollment-1',
        studentNumber: 'S-001',
        studentName: 'Ana Pérez',
        status,
        note: null,
        createdAt: null,
        createdBy: null,
        updatedAt: null,
        updatedBy: null,
      },
    ],
  };
}
