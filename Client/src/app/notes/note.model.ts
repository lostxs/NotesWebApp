import { Tag } from "../tags/tag.model";


export interface Note {
  noteId: number;
  title: string;
  content: string;
  creationDate: Date;
  tags: Tag[]; 
  reminderDate?: Date | null; 
}
