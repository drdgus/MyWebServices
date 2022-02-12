import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserSettings } from "../home/UserSettings";
import { UserPattern } from '../home/UserPattern';

@Component({
  selector: 'app-home',
  templateUrl: './settings.component.html',
})
export class SettingsComponent implements OnInit {

  public userSettings: UserSettings = new UserSettings();
  public selectedPattern: UserPattern = new UserPattern();

  public constructor(private readonly http: HttpClient) { }

  public ngOnInit(): void {
    this.getSettings();
  }

  public onPatternChange()
  {
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

  public saveSettings(): void {
    var requestOptions: object = {
      method: 'PATCH',
      body: this.userSettings,
      redirect: 'follow',
    };

    fetch("https://localhost:8083/api/v1/WordConvert/settings/save", (requestOptions) as any)
      .then(res => console.log("saveSettings response ok: " + res.ok))
      .catch(error => console.log("saveSettings error: " + error));
  }
}
