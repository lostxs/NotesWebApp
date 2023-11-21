import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Note } from '../notes/note.model';
import { Tag } from './tag.model';
import { TagService } from './tag.service';
import { NoteService } from '../notes/notes.service';
import { HttpErrorResponse } from '@angular/common/http';




@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit {
  @Output() notesLoaded = new EventEmitter<void>();
  snackBarMessage: string | null = null;
  notes: Note[] = [];
  tags: Tag[] = [];

  newTag: Tag = {
    tagId: 0,
    name: '',
    notes: []
  };

  selectedNoteId: number = 0;
  selectedTagId: number = 0;

  constructor(private tagService: TagService, private noteService: NoteService) { }


  ngOnInit() {
    this.loadNotes();
    this.loadTags();
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

  loadTags() {
    this.tagService.getTags().subscribe(
      (tags) => {
        this.tags = tags;
      },
      (error) => {
        console.error('Error loading tags:', error);
      }
    );
  }


  assignTagToNote() {
    if (this.selectedNoteId === 0 || this.selectedTagId === 0) {
      this.openSnackBar('Пожалуйста, выберите заметку и тег перед присвоением.');
      console.error('Please select a note and a tag before assigning.');
      return;
    }

    this.tagService.assignTagToNote(this.selectedNoteId, this.selectedTagId).subscribe(
      () => {
        this.openSnackBar('Тег успешно присвоен заметке.');
        console.log('Tag assigned successfully.');
        this.loadTags(); 
      },
      (error) => {
        console.error('Ошибка при присвоении тега:', error);
        console.log(error.error); 
      }
    );
  }

  openSnackBar(message: string) {
    this.snackBarMessage = message;
  }

  closeSnackBar() {
    this.snackBarMessage = null;
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

  addTag() {
    this.tagService.addTag(this.newTag).subscribe(
      (addedTag) => {
        console.log('Tag added successfully:', addedTag);
        this.newTag = {
          tagId: 0,
          name: '',
          notes: []
        }; 

        
        this.loadTags();
      },
      (error) => {
        console.error('Error adding tag:', error);
      }
    );

  }


  deleteTag(tagId: number) {
    const tagIdAsString: string = tagId.toString(); 
    this.tagService.deleteTag(tagIdAsString).subscribe(
      () => {
        console.log('Tag deleted successfully.');
        this.loadTags(); 
      },
      (error) => {
        console.error('Error deleting tag:', error);
      }
    );
  }





}
