
import { Component, OnInit } from '@angular/core';
import { Note } from '../notes/note.model';
import { NoteService } from '../notes/notes.service';
import { Reminder } from './reminder.model';
import { RemindersService } from './reminders.service';

@Component({
  selector: 'app-reminders',
  templateUrl: './reminders.component.html',
  styleUrls: ['./reminders.component.css']
})
export class RemindersComponent implements OnInit {
  notesWithReminders: Note[] = [];
  reminders: Reminder[] = [];
  newReminder: Reminder = {
    reminderId: 0,
    noteId: 0,
    note: { 
      noteId: 0,
      title: '',
      content: '',
      creationDate: new Date(),
      tags: [],
      reminderDate: null
    },
    reminderText: '',
    reminderDate: new Date()
  };

  constructor(private reminderService: RemindersService) { }

  ngOnInit() {
    this.loadReminders();
    this.loadNotesWithReminders();
  }

  loadNotesWithReminders(): void {
    this.reminderService.getNotesWithReminders().subscribe(
      notes => {
        this.notesWithReminders = notes;
      },
      error => {
        console.error('Error loading notes with reminders:', error);
      }
    );
  }

  loadReminders() {
    this.reminderService.getReminders().subscribe(
      (reminders) => {
        this.reminders = reminders;
      },
      (error) => {
        console.error('Error loading reminders:', error);
      }
    );
  }

  addReminder() {
    const newReminder: Reminder = {
      reminderId: 0,
      noteId: this.newReminder.note.noteId, 
      note: this.newReminder.note, 
      reminderText: this.newReminder.reminderText,
      reminderDate: this.newReminder.reminderDate
    };

    this.reminderService.addReminder(newReminder).subscribe(
      (addedReminder) => {
        console.log('Reminder added successfully:', addedReminder);
        this.loadReminders(); 
        this.newReminder = {
          reminderId: 0,
          noteId: 0,
          note: {
            noteId: 0,
            title: '',
            content: '',
            creationDate: new Date(),
            tags: [],
            reminderDate: null
          },
          reminderText: '',
          reminderDate: new Date()
        };
      },
      (error) => {
        console.error('Error adding reminder:', error);
      }
    );
  }
}
