import { CustomUserElement } from "./CustomUserElement";

export class  UserPattern {
  public id: number = 0;
  public name: string = '';
  public customUserElementsForPattern: CustomUserElement[] = new Array<CustomUserElement>();
}
