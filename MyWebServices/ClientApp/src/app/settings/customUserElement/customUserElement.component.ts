import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CustomUserElement} from "../../home/CustomUserElement";
import {SortingOrder} from "../../home/SortingOrder";
import {UserSettingsService} from "../user-settings.service";

@Component({
  selector: 'app-customElements',
  templateUrl: './customUserElement.component.html',
  styleUrls: ['./customUserElement.component.scss']
})

export class CustomUserElementComponent
{
  @Input("customElements")
  set customElements(value: CustomUserElement[]){
    this.dataSource = value;
    this.elements = value
  }

  @Output() elementChange:EventEmitter<CustomUserElement[]> = new EventEmitter<CustomUserElement[]>();

  private elements!: CustomUserElement[];
  public dataSource!: CustomUserElement[];
  public sortingOrders: string[];

  constructor(private userSettingsService: UserSettingsService)
  {
    const orders = Object.keys(SortingOrder);
    this.sortingOrders = orders;
  }

  removeElement(element: CustomUserElement){
    const elements = this.elements;
    const index = elements.indexOf(element, 0);
    if (index > -1) {
      const removedElement = elements.splice(index, 1)[0];
      this.dataSource = elements;
      this.elementChange.emit(this.elements);
      this.userSettingsService.removeCustomElement(removedElement)
    }
  }
}

