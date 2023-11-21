import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Note } from './note.model';
import { NoteService } from './notes.service';
import { TagService } from '../tags/tag.service';
import { TagsComponent } from'../tags/tags.component'
import { Tag } from '../tags/tag.model';
import { MatSelectModule } from '@angular/material/select';


@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrls: ['./notes.component.css']
})
export class NotesComponent implements OnInit {
  @Output() notesLoaded = new EventEmitter<void>();
  snackBarMessage: string | null = null;
  tagsWithNotes: Tag[] = [];
  newNote: Note = {
    noteId: 0,
    title: '',
    content: '',
    creationDate: new Date(),
    tags: [],
    reminderDate: null 
  };
  editingNote: Note | null = null;
  selectedNoteId: number | null = null;
  reminderDateTime: string = '';
  notes: Note[] = [];
  tags: Tag[] = [];
  availableTags: Tag[] = [];
  selectedTags: number[] = []; 
  notesWithReminders: Note[] = []; 
  constructor(private noteService: NoteService, private tagService: TagService) { }

  ngOnInit() {
    this.loadNotes();
    this.loadAvailableTags();
    this.loadNotesWithReminders();
    this.loadTagsWithNotes();
  }

  private loadTagsWithNotes() {
    this.noteService.getTagsWithNotes().subscribe((tags) => {
      this.tagsWithNotes = tags;
    });
  }

  assignTagToNote(noteId: number, tagId: number): void {
    this.noteService.assignTagToNote(noteId, tagId).subscribe(
      () => {
        console.log('Tag assigned successfully.');
      },
      error => {
        console.error('Error assigning tag to note:', error);
      }
    );
  }

  openSnackBar(message: string) {
    this.snackBarMessage = message;
  }

  closeSnackBar() {
    this.snackBarMessage = null;
  }

  loadNotesWithReminders() {
    this.noteService.getNotesWithReminders().subscribe(
      (notes) => {
        this.notes = notes;
        this.getReminders(); 
      },
      (error) => {
        console.error('Error loading notes with reminders:', error);
      }
    );
  }


  getReminders() {
    this.notesWithReminders = this.notes.filter(note => note.reminderDate !== null);
  }

  loadNotes() {
    this.noteService.getNotes().subscribe(
      (data) => {
        this.notes = data;
      },
      (error) => {
        console.error('Failed to load notes', error);
      }
    );
    this.notesLoaded.emit();
  }


  startEditing(note: Note) {
    this.editingNote = { ...note }; 
    this.selectedNoteId = note.noteId;
  }

  cancelEditing() {
    this.editingNote = null; 
  }



  getNotes() {
    this.noteService.getNotes().subscribe(
      (notes) => {
        console.log('Fetched notes:', notes);
        this.notes = notes;
      },
      (error) => {
        console.error('Error fetching notes:', error);
      }
    );
  }




  loadAvailableTags() {
    this.tagService.getTags().subscribe(
      (tags) => {
        this.availableTags = tags;
      },
      (error) => {
        console.error('Error loading tags:', error);
      }
    );
  }

  setReminder(noteId: number, reminderDateTime: Date): void {
    this.noteService.setReminder(noteId, reminderDateTime).subscribe(
      (response) => {
        console.log(response.message); 
      },
      (error) => {
        console.error('Error setting reminder:', error);
      }
    );
  }



  addNoteWithReminder(): void {
    this.newNote.reminderDate = new Date(this.reminderDateTime);

    this.noteService.addNote(this.newNote).subscribe(
      (addedNote) => {
        this.openSnackBar('Заметка успешно добавлена.');
        console.log('Note added successfully:', addedNote);
        this.loadNotes(); 
        this.newNote = { noteId: 0, title: '', content: '', creationDate: new Date(), tags: [] }; 
        this.selectedTags = []; 
        this.reminderDateTime = '';
      },
      (error) => {
        this.openSnackBar('Пожалуйста, введите название заметки.');
        console.error('Error adding note:', error);
      }
    );
  }

  addNote(): void {
    this.noteService.addNote(this.newNote)
      .subscribe(
        response => {
          console.log(response);
          this.newNote = {
            noteId: 0,
            title: '',
            content: '',
            creationDate: new Date(),
            tags: [],
            reminderDate: null
          };
        },
        error => {
          console.error(error);
        }
      );

    this.noteService.addNote(this.newNote).subscribe(
      (addedNote) => {
        console.log('Note added successfully:', addedNote);
        this.loadNotes(); 
        this.newNote = { noteId: 0, title: '', content: '', creationDate: new Date(), tags: []}; 
        this.selectedTags = []; 
      },
      (error) => {
        console.error('Error adding note:', error);
      }
    );
  }

  updateNote() {
    console.log('Editing note before update:', this.editingNote);
    if (this.editingNote?.noteId !== undefined) { 
      this.noteService.updateNote(this.editingNote).subscribe(
        (updatedNote) => {
          console.log('Note updated successfully:', updatedNote);
          this.loadNotes(); 
          this.editingNote = null; 
        },
        (error) => {
          console.error('Error updating note:', error);
        }
      );
    } else {
      console.error('Invalid noteId in editingNote:', this.editingNote);
    }
  }

  editNote(note: Note) {
    console.log('Editing note:', note);

    if (note.noteId !== undefined) {
      this.editingNote = { ...note }; 
    } else {
      console.error('Invalid noteId in note:', note);
    }
  }


  saveEditedNote() {
    this.noteService.updateNote({
      noteId: this.editingNote!.noteId,
    }).subscribe(
      (updatedNote) => {
        console.log('Note updated successfully:', updatedNote);
        this.editingNote = null; 
      },
      (error) => {
        console.error('Error updating note:', error);
      }
    );
  }

  deleteNote(id: number) {
    this.noteService.deleteNote(id).subscribe(
      () => {
        console.log('Note deleted successfully.');
        this.loadNotes(); 
      },
      (error) => {
        console.error('Error deleting note:', error);
      }
    );
  }


  getTagsForNote(note: any): string[] {
    const matchingTag = this.tagsWithNotes.find(tag => tag.notes.some(n => n.noteId === note.noteId));

    return matchingTag ? [matchingTag.name] : [];
  }








}
