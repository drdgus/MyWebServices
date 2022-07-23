import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserPattern } from '../home/UserPattern';
import {MatSnackBar} from "@angular/material/snack-bar";
import {UserSettingsService} from "./user-settings.service";
import {UserSettings} from "../home/UserSettings";
import {CustomUserElement} from "../home/CustomUserElement";
import {SortingOrder} from "../home/SortingOrder";

@Component({
  selector: 'app-home',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit
{
  public userSettings: UserSettings =  new UserSettings();
  public selectedPatternId: number = 0;
  public selectedPattern: UserPattern = new UserPattern();

  public constructor(private readonly http: HttpClient, private snackBar: MatSnackBar, private userSettingsService: UserSettingsService) {
    userSettingsService.getSettings().then(settings => {
      this.userSettings = settings;
      this.selectedPattern = this.userSettings.userPatterns[0];
      this.selectedPatternId = this.selectedPattern.id;
    })
  }

  public ngOnInit(): void
  {

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

  public saveSettings(): void
  {
    this.userSettingsService.saveSettings()
      .subscribe(() => this.openInfoSnackBar("Настройки сохранены."),
          error => {
            console.log("saveSettings error: " + JSON.stringify(error))
            this.openErrorSnackBar("Ошибка сохранения настроек.")
      });
  }

  addSharedElement() {
    const element = new CustomUserElement();
    element.elementSortingOrder = SortingOrder.BeforeText;
    this.userSettings.sharedCustomUserElements.push(element);
    this.userSettings.sharedCustomUserElements = new Array<CustomUserElement>().concat(this.userSettings.sharedCustomUserElements);
    this.userSettingsService.addCustomElement(element);
  }

  addElementForPattern() {
    const selectedPattern = this.userSettings.userPatterns.find(p => p.id == this.selectedPatternId)!;
    const newElement = new CustomUserElement();
    newElement.elementSortingOrder = SortingOrder.BeforeText;
    newElement.userPatternId = this.selectedPatternId;
    selectedPattern.customUserElementsForPattern.push(newElement)
    selectedPattern.customUserElementsForPattern = new Array<CustomUserElement>().concat(selectedPattern.customUserElementsForPattern);
    this.userSettingsService.addCustomElement(newElement);
  }

  elementChanged($event: CustomUserElement[], patternId: number) {
    if(patternId == -1){
      this.userSettings.sharedCustomUserElements = $event;
    }
    else{
      const pattern = this.userSettings.userPatterns.find(p => p.id == this.selectedPatternId)!;
      pattern.customUserElementsForPattern = $event;
    }
  }
}
