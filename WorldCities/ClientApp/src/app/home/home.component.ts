import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
   isOnline: boolean;
  ngOnInit() {
    this.isOnline=navigator.onLine;
 }
}
function ngOnInit() {
  
}

