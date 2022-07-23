import {Component, Input, OnInit} from '@angular/core';
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-converted-text',
  templateUrl: './converted-text.component.html',
  styleUrls: ['./converted-text.component.css']
})
export class ConvertedTextComponent implements OnInit {

  public renderedHtml: boolean = false;
  public styles: string = "";

  @Input("text") text!: string;
  constructor(private snackBar: MatSnackBar) {
    this.addStyles();
  }

  ngOnInit(): void {
  }

  private addStyles():void {
    const head = document.getElementsByTagName('HEAD')[0];
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = 'StylesForRenderedHTML.css';

    head.appendChild(link);
  }

  public copyText() {
    navigator.clipboard.writeText(this.text);
    this.openInfoSnackBar("Текст скопирован.");
  }

  private openInfoSnackBar(message: string) {
    this.snackBar.open(message)._dismissAfter(3 * 1000);
  }

  showConvertedHtml() {
    this.renderedHtml = !this.renderedHtml;
  }
}
