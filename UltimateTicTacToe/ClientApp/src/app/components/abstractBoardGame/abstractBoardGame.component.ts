import { Player } from "../../models/player/player.model";
import { Input, Output, EventEmitter } from "@angular/core";
import { BoardGame } from "../../models/boardGame/boardgame/boardgame.model";

export abstract class AbstractBoardGameComponent  {

    @Input() owner: Player;
    @Input() board: Array<Array<BoardGame>>;
    @Output() moveEvent =  new EventEmitter();


}
