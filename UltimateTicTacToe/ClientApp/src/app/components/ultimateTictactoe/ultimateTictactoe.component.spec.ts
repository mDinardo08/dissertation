import { TictactoeComponent } from "./ultimateTictactoe.component";
import { Move } from "../../models/move/move.model";

describe("Tictactoe component", () => {

    let comp: TictactoeComponent;

    beforeEach(() => {
        comp = new TictactoeComponent();
    });

    it("Will emit a move event when it recieves one", () => {
        spyOn(comp.moveEvent, "emit");
        comp.moveMade(null, null, null);
        expect(comp.moveEvent.emit).toHaveBeenCalled();
    });

    it("Will emit a move object", () => {
        spyOn(comp.moveEvent, "emit");
        comp.moveMade(null, null, null);
        expect(comp.moveEvent.emit).toHaveBeenCalledWith(jasmine.any(Move));
    });

    it("Will emit a move whoes next property is the event it recieved", () => {
        spyOn(comp.moveEvent, "emit");
        const move = new Move();
        comp.moveMade(move, null, null);
        const event = new Move();
        event.next = move;
        event.possition = {
            X: null, Y: null
        };
        expect(comp.moveEvent.emit).toHaveBeenCalledWith(event);
    });

    it("Will assign a possition with the x being equal to the x passed in", ()  => {
        spyOn(comp.moveEvent, "emit");
        const move = new Move();
        comp.moveMade(null, 0, null);
        const event = new Move();
        event.possition = {X: 0, Y: null};
        event.next = null;
        expect(comp.moveEvent.emit).toHaveBeenCalledWith(event);
    });

    it("Will assign a possition with the y being equal to the y passed in", ()  => {
        spyOn(comp.moveEvent, "emit");
        const move = new Move();
        comp.moveMade(null, null, 0);
        const event = new Move();
        event.possition = {X: null, Y: 0};
        event.next = null;
        expect(comp.moveEvent.emit).toHaveBeenCalledWith(event);
    });
});
