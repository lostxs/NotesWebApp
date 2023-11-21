import { Note } from "../notes/note.model";

export interface Tag {
  tagId: number;
  name: string;
  notes: Note[];
}
