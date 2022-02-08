import { CustomUserElement } from "./CustomUserElement";

export class  UserPattern {
  public Id: number = 0;
  public Name: string = '';
  public CustomUserElementsForPattern: CustomUserElement[] = new Array<CustomUserElement>();
}
