import { CustomUserElement } from "./CustomUserElement";
import { UserPattern } from "./UserPattern";

export class UserSettings {

  public userPatterns: UserPattern[] = new Array <UserPattern>();
  public SharedCustomUserElements: CustomUserElement[] = new Array<CustomUserElement>();

  public TextLengthBeforeCut: number = 0;
  public CutElement: string = '';

  public ParagraphElement: string = '';
  public ParagraphCenterAlignClass: string = '';

  public ListElement: string = '';
}
