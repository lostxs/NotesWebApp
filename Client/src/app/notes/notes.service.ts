import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { Note } from './note.model';
import { Tag } from '../tags/tag.model';

@Injectable({
  providedIn: 'root',
})


export class NoteService {


  private apiUrl = 'https://localhost:7155/api/notes';

  private notesLoadedSubject = new Subject<void>();

  notesLoaded$ = this.notesLoadedSubject.asObservable();

  constructor(private http: HttpClient) { }

  getNotes(): Observable<Note[]> {
    return this.http.get<Note[]>(this.apiUrl); 
  }

  getNotesWithReminders(): Observable<Note[]> {
    return this.http.get<Note[]>(this.apiUrl);
  }

  addNote(newNote: Note): Observable<Note> {
    return this.http.post<Note>(this.apiUrl, newNote);
  }

  updateNote(val: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${val.noteId}`, val);
  }

  deleteNote(noteId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${noteId}`);
  }

  assignTagToNote(noteId: number, tagId: number): Observable<void> {
    const url = `${this.apiUrl}/assigntag`;
    const model = { noteId, tagId };
    return this.http.post<void>(url, model);
  }

  getTagsWithNotes(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/tags-with-notes`);
  }

  notifyNotesLoaded() {
    this.notesLoadedSubject.next();
  }

  setReminder(noteId: number, reminderDateTime: Date): Observable<any> {
    const url = `${this.apiUrl}/${noteId}/set-reminder`;
    const body = { reminderDateTime };

    return this.http.post(url, body);
  }

}
