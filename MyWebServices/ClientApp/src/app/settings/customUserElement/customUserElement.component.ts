import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CustomUserElement} from "../../home/CustomUserElement";
import {SortingOrder} from "../../home/SortingOrder";
import {MatTableDataSource} from "@angular/material/table";

@Component({
  selector: 'app-customElements',
  templateUrl: './customUserElement.component.html',
  styleUrls: ['./customUserElement.component.scss']
})

export class CustomUserElementComponent
{
  @Input("customElements")
  set customElements(value: CustomUserElement[]){
    this.dataSource = new MatTableDataSource<CustomUserElement>(value);
    this.elements = value
  }

  @Output() elementChange:EventEmitter<CustomUserElement[]> = new EventEmitter<CustomUserElement[]>();

  private elements!: CustomUserElement[];
  public displayedColumns: string[] = ['name', 'value', 'templateValue', 'elementSortingOrder', 'del'];
  public dataSource!: MatTableDataSource<CustomUserElement>;
  public sortingOrders: string[];

  constructor()
  {
    const orders = Object.keys(SortingOrder);
    this.sortingOrders = orders;
  }

  removeElement(element: CustomUserElement){
    const elements = this.elements;
    const index = elements.indexOf(element, 0);
    if (index > -1) {
      elements.splice(index, 1);
      this.dataSource = new MatTableDataSource<CustomUserElement>(elements);
    }

    this.elementChange.emit(this.elements);
  }
}

