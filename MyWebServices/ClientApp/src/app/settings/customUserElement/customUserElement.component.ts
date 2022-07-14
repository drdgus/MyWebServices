import {Component, Input } from '@angular/core';
import {CustomUserElement} from "../../home/CustomUserElement";
import {SortingOrder} from "../../home/SortingOrder";

@Component({
  selector: 'app-customElements',
  templateUrl: './customUserElement.component.html',
  styleUrls: ['./customUserElement.component.scss'],
  inputs: ['element']
})

export class CustomUserElementComponent
{
  @Input("elements") elements!: CustomUserElement[];

  public displayedColumns: string[] = ['name', 'value', 'templateValue', 'elementSortingOrder'];
  sortingOrders: any = Object.keys(SortingOrder).slice(0, Object.keys(SortingOrder).length / 2);

  constructor(){ }
}

