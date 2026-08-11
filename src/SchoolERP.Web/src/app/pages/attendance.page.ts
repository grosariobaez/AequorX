import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import {
  AttendanceRoster,
  AttendanceStudent,
  EffectiveAttendanceStatus,
  Section,
} from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

interface AttendanceEdit extends AttendanceStudent {
  originalStatus: EffectiveAttendanceStatus;
  originalNote: string | null;
}

@Component({
  imports: [FormsModule],
  templateUrl: './attendance.page.html',
})
export class AttendancePage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly sections = signal<Section[]>([]);
  protected readonly students = signal<AttendanceEdit[]>([]);
  protected readonly error = signal<string | null>(null);
  protected readonly confirmation = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected date = localDate();
  protected sectionId = '';

  protected readonly statuses: EffectiveAttendanceStatus[] = [
    'Present',
    'Absent',
    'Late',
    'Excused',
    'EarlyDeparture',
  ];

  constructor() {
    this.api.get<Section[]>('/api/sections').subscribe({
      next: (sections) => this.sections.set(sections),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected load(): void {
    if (!this.sectionId || !this.date) {
      return;
    }

    this.error.set(null);
    this.confirmation.set(null);
    this.api
      .getWithParams<AttendanceRoster>('/api/attendance', {
        sectionId: this.sectionId,
        date: this.date,
      })
      .subscribe({
        next: (roster) => this.students.set(roster.students.map(toEdit)),
        error: () => this.error.set(this.i18n.text('loadError')),
      });
  }

  protected save(): void {
    const changed = this.students().filter(
      (student) =>
        student.status !== student.originalStatus ||
        normalizeNote(student.note) !== normalizeNote(student.originalNote),
    );

    if (changed.length === 0) {
      this.confirmation.set(this.i18n.text('attendanceSaved'));
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.confirmation.set(null);

    forkJoin(
      changed.map((student) =>
        this.api.put<AttendanceStudent>(
          `/api/attendance/${student.enrollmentId}/${this.date}`,
          { status: student.status, note: normalizeNote(student.note) },
        ),
      ),
    ).subscribe({
      next: () => {
        this.saving.set(false);
        this.load();
        this.confirmation.set(this.i18n.text('attendanceSaved'));
      },
      error: () => {
        this.saving.set(false);
        this.error.set(this.i18n.text('saveError'));
      },
    });
  }
}

function toEdit(student: AttendanceStudent): AttendanceEdit {
  return {
    ...student,
    originalStatus: student.status,
    originalNote: student.note,
  };
}

function normalizeNote(note: string | null): string | null {
  const normalized = note?.trim();
  return normalized ? normalized : null;
}

function localDate(): string {
  const date = new Date();
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}
