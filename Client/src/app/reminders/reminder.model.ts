import { Note } from "../notes/note.model";


export interface Reminder {
  reminderId: number;
  noteId: number;
  note: Note; 
  reminderText: string;
  reminderDate: Date;
 
}
