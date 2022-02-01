import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public isFileEmpty: boolean = true;
  public isProcessing: boolean = false;

  public fileName: string = "";
  public convertedText: string = "";
  private _file: File = new File([], "");

  public constructor(private http: HttpClient) { }

  private ngOnInit(): void { }

  public async onFileSelected(event: any): Promise<void> {

    this._file = event.target.files[0];

    if (this._file) {

      this.isFileEmpty = false;
      this.fileName = this._file.name;
      console.log(`openedFile: ${this.fileName}`);
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

    fetch("https://localhost:7078/api/v1/WordConvert/process-file", requestOptions)
      .then(response => response.text())
      .then(result => this.convertedText = result)
      .catch(error => console.log('error', error));
  }
}
