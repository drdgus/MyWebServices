import {Component, Input, OnInit} from '@angular/core';
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-converted-text',
  templateUrl: './converted-text.component.html',
  styleUrls: ['./converted-text.component.css']
})
export class ConvertedTextComponent implements OnInit {

  public renderedHtml: boolean = false;

  @Input("text") text: string = "";
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

  public copyText():void {
    if (window.isSecureContext && navigator.clipboard) {
      navigator.clipboard.writeText(this.text);
    } else {
      this.unsecuredCopyToClipboard();
    }

    this.openInfoSnackBar("Текст скопирован.");
  }

  private unsecuredCopyToClipboard():void {
    const textArea = document.createElement("textarea");
    textArea.value = this.text;
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    try {
      document.execCommand('copy');
    } catch (err) {
      console.error('Unable to copy to clipboard', err);
    }
    document.body.removeChild(textArea);
  }

  private openInfoSnackBar(message: string) {
    this.snackBar.open(message)._dismissAfter(3 * 1000);
  }

  showConvertedHtml() {
    this.renderedHtml = !this.renderedHtml;
  }
}
