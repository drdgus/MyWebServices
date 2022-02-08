import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';
import { UserPattern } from './UserPattern';
import { UserSettings } from './UserSettings';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent {

  public userSettings: UserSettings = new UserSettings();
  public userPatterns: UserPattern[];

  public isFileEmpty: boolean = true;
  public isProcessing: boolean = false;

  public fileName: string = "";
  public convertedText: string = "";

  private _file: File = new File([], "");

  public constructor(private http: HttpClient)
  {
    this.userPatterns = this.userSettings.userPatterns;
  }

  private ngOnInit(): void {
    this.getSettings();
  }

  public async onFileSelected(event: any): Promise<void> {

    this.getSettings();
    this._file = event.target.files[0];

    if (this._file) {

      this.isFileEmpty = false;
      this.fileName = this._file.name;
      console.log(`openedFile: ${this.fileName}`);
    }
    else {
      console.log('error', this._file);
    }
  }

  public processFile(): void {
    this.isProcessing = true;

    this.ConvertText();

    this.isProcessing = false;
    this.isFileEmpty = true;
  }

  private ConvertText(): void {

    let formData = new FormData();
    formData.append("file", this._file);

    var requestOptions: object = {
      method: 'POST',
      body: formData,
      redirect: 'follow',
    };

    fetch("https://localhost:8083/api/v1/WordConvert/process-file", requestOptions)
      .then(response => response.text())
      .then(result => this.convertedText = result)
      .catch(error => console.log('error', error));
  }

  private getSettings(): void {

    var requestOptions: object = {
      method: 'GET',
      redirect: 'follow',
    };

    fetch("https://localhost:8083/api/v1/WordConvert/settings", requestOptions)
      .then(response => response.json())
      .then((json: UserSettings) => {
        console.log(JSON.stringify(json));
        this.userSettings = json;
      })
      .catch(error => console.log('error', error));
  }

  private saveSettings(): void {

  }
}
