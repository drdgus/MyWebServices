import {Injectable} from '@angular/core';
import {UserSettings} from "../home/UserSettings";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {CustomUserElement} from "../home/CustomUserElement";
import {SortingOrder} from "../home/SortingOrder";

@Injectable({
  providedIn: 'root'
})
export class UserSettingsService {
  private userSettings: UserSettings | null = null;
  private addedCustomElements: CustomUserElement[] = [];
  private removedCustomElements: CustomUserElement[] = [];

  constructor(private http: HttpClient) {
  }

  public async getSettings() {
    if(this.userSettings === null){
      const res = await this.http.get<UserSettings>("/api/v1/WordConvert/settings").toPromise();
      this.userSettings = res;
      return res;
    }
    return this.userSettings;
  }

  public removeCustomElement(element: CustomUserElement):void {
    if(element.name === "" || element.value === "") return;
    if(this.addedCustomElements.find(i => i == element)){
      const index = this.addedCustomElements.indexOf(element, 0);
      if (index > -1) {
        this.addedCustomElements.splice(index, 1)
      }
    }
    else this.removedCustomElements.push(element);
  }

  public addCustomElement(element: CustomUserElement):void {
    this.addedCustomElements.push(element);
  }

  public saveSettings(): Observable<any>{

    const settings = new UserSettings();
    settings.cutElement = this.userSettings!.cutElement;
    settings.textLengthBeforeCut = this.userSettings!.textLengthBeforeCut;
    settings.paragraphElement = this.userSettings!.paragraphElement;
    settings.paragraphCenterAlignClass = this.userSettings!.paragraphCenterAlignClass;
    settings.listElement = this.userSettings!.listElement;

    const updatedSettings = {
      settings,
      addedElements: this.addedCustomElements,
      removedElements: this.removedCustomElements,
      updatedElements: this.getUpdatedElements()
    }

    console.log("send settings", updatedSettings);
    return this.http.patch("/api/v1/WordConvert/settings/save", updatedSettings);
  }

  private getUpdatedElements(): CustomUserElement[]{
    var items = Array<CustomUserElement>();
    items = items.concat(this.userSettings!.sharedCustomUserElements
      .filter(sharedItem => sharedItem.id != 0 &&
        this.removedCustomElements
          .find(removedItem => removedItem.id === sharedItem.id) === undefined));

    for(let i = 0; i < this.userSettings!.userPatterns.length; i++){
      items = items.concat(this.userSettings!.userPatterns[i].customUserElementsForPattern
        .filter(patternItem => patternItem.id != 0 &&
          this.removedCustomElements.find(removedItem => removedItem.id === patternItem.id) === undefined));
    }

    console.log(items);
    return items;
  }
}
