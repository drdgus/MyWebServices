import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserSettings } from "../home/UserSettings";
import { UserPattern } from '../home/UserPattern';
import {MatSnackBar} from "@angular/material/snack-bar";
import {SortingOrder} from "../home/SortingOrder";
import {EnumValue} from "@angular/compiler-cli/src/ngtsc/partial_evaluator";
import {CustomUserElement} from "../home/CustomUserElement";

@Component({
  selector: 'app-home',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit
{
  public userSettings: UserSettings = new UserSettings();
  public selectedPatternId: number = 0;
  public selectedPattern: UserPattern = new UserPattern();

  public constructor(private readonly http: HttpClient, private snackBar: MatSnackBar) {}

  public ngOnInit(): void
  {
    this.getSettings();
  }

  private openErrorSnackBar(message: string) {
    this.snackBar.open(message, 'Закрыть');
  }

  private openInfoSnackBar(message: string) {
    this.snackBar.open(message)._dismissAfter(3 * 1000);
  }

  public onPatternChange()
  {
    this.selectedPattern = this.userSettings.userPatterns.find(p => p.id == this.selectedPatternId)!;
  }

  private getSettings(): void
  {
    this.http.get<UserSettings>("/api/v1/WordConvert/settings").subscribe(settings =>
    {
      this.userSettings = settings;
      this.selectedPattern = this.userSettings.userPatterns[0];
    });
  }

  public saveSettings(): void
  {
    // @ts-ignore (поле типа number превращается в sting после обновления значения в Select)
    this.userSettings.sharedCustomUserElements.forEach(i => i.elementSortingOrder = parseInt(i.elementSortingOrder))

    this.http.patch("/api/v1/WordConvert/settings/save", this.userSettings)
      .subscribe(next => this.openInfoSnackBar("Настройки сохранены."),
          error => {
            console.log("saveSettings error: " + JSON.stringify(error))
            this.openErrorSnackBar("Ошибка сохранения настроек.")
      });
  }

  addSharedElement() {
    this.userSettings.sharedCustomUserElements.push(new CustomUserElement());
    this.userSettings.sharedCustomUserElements = new Array<CustomUserElement>().concat(this.userSettings.sharedCustomUserElements);
  }

  addElementForPattern() {
    const newElement = new CustomUserElement();
    newElement.userPatternId = this.selectedPatternId;
    this.selectedPattern.customUserElementsForPattern.push(newElement)
    this.selectedPattern.customUserElementsForPattern = new Array<CustomUserElement>().concat(this.selectedPattern.customUserElementsForPattern);
    console.log(this.selectedPattern.customUserElementsForPattern);
  }
}
