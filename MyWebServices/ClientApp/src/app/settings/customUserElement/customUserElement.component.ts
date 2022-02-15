import { Component, OnInit, Input } from '@angular/core';
import {CustomUserElement} from "../../home/CustomUserElement";

@Component({
  selector: 'customUserElement',
  templateUrl: './customUserElement.component.html',
  inputs: ['element']
})

export class CustomUserElementComponent
{
  @Input()
  public element: CustomUserElement = new CustomUserElement();

  constructor() {  }
}

