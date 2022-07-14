import {Component, Input, OnInit} from '@angular/core';
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-converted-text',
  templateUrl: './converted-text.component.html',
  styleUrls: ['./converted-text.component.css']
})
export class ConvertedTextComponent implements OnInit {

  @Input("text") text!: string;
  constructor(private snackBar: MatSnackBar) { }

  ngOnInit(): void {
  }

  public copyText() {
    navigator.clipboard.writeText(this.text);
    this.openInfoSnackBar("Текст скопирован.");
  }

  private openInfoSnackBar(message: string) {
    this.snackBar.open(message)._dismissAfter(3 * 1000);
  }
}
