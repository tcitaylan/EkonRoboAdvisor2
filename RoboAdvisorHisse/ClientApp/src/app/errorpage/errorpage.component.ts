import { Component, OnInit, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-errorpage',
  templateUrl: './errorpage.component.html',
  styleUrls: ['./errorpage.component.css']
})
export class ErrorpageComponent implements OnInit {

  constructor(private render: Renderer2) {}

  ngOnInit(): void {
    this.render.addClass(document.getElementById('popupoverlay'), 'hide');

  }

  






}
