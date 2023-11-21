import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tag } from './tag.model';


@Injectable({
  providedIn: 'root',
})
export class TagService {

  private apiUrl = 'https://localhost:7155/api/tags';
  private apiUrl2 = 'https://localhost:7155/api/notes';

  constructor(private http: HttpClient) { }

  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(this.apiUrl);
  }

  addTag(val: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, val);
  }

  deleteTag(tag: string): Observable<void> {
    const url = `${this.apiUrl}/${tag}`;
    return this.http.delete<void>(url);
  }

  assignTagToNote(noteId: number, tagId: number): Observable<void> {
    const url = `${this.apiUrl2}/assigntag`;
    const model = { noteId, tagId };
    return this.http.post<void>(url, model);
  }
}
