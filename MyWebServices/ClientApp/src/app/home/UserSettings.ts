import { CustomUserElement } from "./CustomUserElement";
import { UserPattern } from "./UserPattern";

export class UserSettings {

  public userPatterns: UserPattern[] = new Array <UserPattern>();
  public sharedCustomUserElements: CustomUserElement[] = new Array<CustomUserElement>();

  public textLengthBeforeCut: number = 0;
  public cutElement: string = '';

  public paragraphElement: string = '';
  public paragraphCenterAlignClass: string = '';

  public listElement: string = '';
}
