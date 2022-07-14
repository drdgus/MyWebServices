import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { UserSettings } from './UserSettings';
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html"
})
export class HomeComponent implements OnInit {

  public userSettings: UserSettings = new UserSettings();

  public isFileEmpty: boolean = true;
  public isProcessing: boolean = false;
  public fileName: string = "";
  public selectedPatternId: number = 0;

  private file: File = new File([], "");

  public convertedText: string = "";

  public constructor(private readonly http: HttpClient, private snackBar: MatSnackBar) { }

  public async ngOnInit() {
    this.getSettings();
  }

  public async onFileSelected(event: any): Promise<void> {
    this.file = event.target.files[0];
    if (this.file) {
      if (!this.file.type.includes("doc") && !this.file.type.includes("docx")) {
        this.openErrorSnackBar("Неверный формат файла.")
        return;
      }

      this.isFileEmpty = false;
      this.fileName = this.file.name;

      this.openInfoSnackBar("Файл выбран.");
    }
    else console.log("error", this.file);
  }

  private openErrorSnackBar(message: string) {
    this.snackBar.open(message, 'Закрыть');
  }

  private openInfoSnackBar(message: string) {
    this.snackBar.open(message)._dismissAfter(3 * 1000);
  }

  public onPatternChange(): void {
    if (this.file.size !== 0) this.isFileEmpty = false;
  }

  public processFile(): void {
    this.isProcessing = true;
    this.convertedText = "";

    this.openInfoSnackBar("Обработка файла...");
    this.convertText();
  }

  private convertText(): void {
    const formData = new FormData();
    formData.append("file", this.file);

    this.http.post("/api/v1/WordConvert/process-file/" + this.selectedPatternId, formData, {responseType: "text"}).subscribe(next => {
      this.convertedText = next;
      this.isProcessing = false;
      this.isFileEmpty = true;

      this.openInfoSnackBar("Файл обработан.");
    }, error => {
      console.log(JSON.stringify(error));
      this.openErrorSnackBar("Ошибка при обработке файла.");
      this.isProcessing = false;
      this.isFileEmpty = false;
    });
  }

  private getSettings(): void {
    this.http.get<UserSettings>("/api/v1/WordConvert/settings").subscribe(next => {
      this.userSettings = next;
      this.selectedPatternId = this.userSettings.userPatterns[0].id;
    });
  }
}
