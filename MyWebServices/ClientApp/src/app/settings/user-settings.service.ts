import {Injectable} from '@angular/core';
import {UserSettings} from "../home/UserSettings";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UserSettingsService {
  constructor(private http: HttpClient) {
  }

  public getSettings() {
    return this.http.get<UserSettings>("/api/v1/WordConvert/settings");
  }
}
