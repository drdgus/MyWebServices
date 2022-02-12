import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, Observer } from "rxjs";
import { UserPattern } from './UserPattern';
import { UserSettings } from './UserSettings';

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html"
})
export class HomeComponent implements OnInit {
  public userSettings: UserSettings = new UserSettings();

  public isFileEmpty: boolean = true;
  public isProcessing: boolean = false;

  public fileName: string = "";
  public convertedText: string = "";

  public selectedPatternId: number = 0;

  public info: string = "";

  private file: File = new File([], "");

  public constructor(private readonly http: HttpClient) { }

  public async ngOnInit() {
    this.getSettings();
  }

  public async onFileSelected(event: any): Promise<void> {
    this.file = event.target.files[0];
    console.log(this.file.name);
    if (this.file) {
      if (this.file.type.includes("doc") === false && this.file.type.includes("docx") === false) {
        this.info = "Неверный формат файла.";
        setInterval(() => this.info = "", 3000);
        return;
      }

      this.info = "";

      this.isFileEmpty = false;
      this.fileName = this.file.name;

    }
    else console.log("error", this.file);
  }

  public onPatternChange(): void {
    if (this.file.size !== 0) this.isFileEmpty = false;
  }

  public processFile(): void {
    this.isProcessing = true;
    this.convertedText = "";

    this.convertText();

    this.isProcessing = false;
    this.isFileEmpty = true;
  }

  private convertText(): void {
    const formData = new FormData();
    formData.append("file", this.file);

    const requestOptions: object = {
      method: "POST",
      body: formData,
      redirect: "follow"
    };

    fetch("https://localhost:8083/api/v1/WordConvert/process-file/" + this.selectedPatternId, (requestOptions) as any)
      .then(response => response.text())
      .then(result => this.convertedText = result)
      .catch(error => console.log('error', error));
  }

  private getSettings(): void {
    var requestOptions: object = {
      method: 'GET',
      redirect: 'follow',
    };

    fetch("https://localhost:8083/api/v1/WordConvert/settings", (requestOptions) as any)
      .then(text => text.json())
      .then((settings: UserSettings) => {
        this.userSettings = settings;
      });
  }

  private saveSettings(): void {
  }
}
