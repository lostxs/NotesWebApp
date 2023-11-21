
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reminder } from './reminder.model';
import { Note } from '../notes/note.model';

@Injectable({
  providedIn: 'root'
})
export class RemindersService {
  private apiUrl = 'https://localhost:7155/api/reminders';
  private apiUrl2 = 'https://localhost:7155/api/notes';

  constructor(private http: HttpClient) { }

  getReminders(): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(this.apiUrl);
  }

  addReminder(val: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, val);
  }

  getNotesWithReminders(): Observable<Note[]> {
    const url = `${this.apiUrl2}/notes-with-reminders`;
    return this.http.get<Note[]>(url);
  }


}
