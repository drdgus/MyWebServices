import {SortingOrder} from "./SortingOrder";

export class CustomUserElement {
  public id: number = 0;
  public name: string = '';
  public value: string = '';
  public templateValue: string | null = null;
  public elementSortingOrder: number = 0;
  public userPatternId: number | null = null;

  public withoutTemplate: boolean = (this.templateValue == null || this.templateValue?.length === 0 );
}

