import { GameService } from "./game.service";
import { Observable } from "rxjs/Observable";
import { BoardGameDTO } from "../../models/DTOs/BoardGameDTO";
import { BoardGame } from "../../models/boardGame/boardgame/boardgame.model";
import { BoardCreationDTO } from "../../models/DTOs/BoardCreationDTO";
import { Player } from "../../models/player/player.model";
import { Move } from "../../models/move/move.model";
import { PlayerType } from "../../models/player/player.type.enum";
import { RatingDTO } from "../../models/DTOs/RatingDTO";
import { PlayerColour } from "../../models/player/player.colour.enum";

describe("Game Service tests", () => {

    let service: GameService;

    beforeEach(() => {
        service = new GameService(null);
        service.availableMoves = [];
    });

    it("Will assign the current players colour as the owner of the tile described on the move", () => {
        const board = new Array<Array<BoardGame>>();
        board[0] = new Array<BoardGame>();
        board[0][0] = new BoardGame();
        service.board = board;
        const move = new Move();
        move.possition = {x: 0, y: 0};
        move.next = new Move();
        move.owner = 100;
        const result = service.makeMoveOnBoard(board, move);
        expect(result[0][0].owner).toBe(100);
    });

    it("Will assign the current player of the owner of a tile across a row", () => {
        const board = new Array<Array<BoardGame>>();
        board[0] = new Array<BoardGame>();
        board[0][0] = new BoardGame();
        board[0][1] = new BoardGame();
        service.board = board;
        const move = new Move();
        move.possition = {x: 0, y: 1};
        move.next = new Move();
        move.owner = 0;
        const result = service.makeMoveOnBoard(board, move);
        expect(result[0][1].owner).toBe(0);
    });

    it("Will assign the current player as the owner of a tile down a column", () => {
        const board = new Array<Array<BoardGame>>();
        board[0] = new Array<BoardGame>();
        board[1] = new Array<BoardGame>();
        board[1][0] = new BoardGame();
        service.board = board;
        const move = new Move();
        move.next = new Move();
        move.possition = {x: 1, y: 0};
        move.owner = 100;
        const result = service.makeMoveOnBoard(board, move);
        expect(result[1][0].owner).toBe(100);
    });

    it("Will call to make a move with nested move", () => {
        const board = new Array<Array<BoardGame>>();
        board[0] = new Array<BoardGame>();
        const innerBoard = new BoardGame();
        innerBoard.board = new Array<Array<BoardGame>>();
        innerBoard.board[0] = new Array<BoardGame>();
        innerBoard.board[0][0] = new BoardGame();
        board[0][0] = innerBoard;
        service.board = board;
        const move = new Move();
        move.possition = {x: 0, y: 0};
        const innerMove = new Move();
        innerMove.possition = {x: 0, y: 0};
        innerMove.next = new Move();
        move.next = innerMove;
        service.curPlayer = new Player();
        spyOn(service, "makeMoveOnBoard").and.callThrough();
        service.makeMoveOnBoard(board, move);
        expect(service.makeMoveOnBoard).toHaveBeenCalledTimes(2);
    });

    it("Will call the api service to make a move", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.cur = new Player();
        dto.cur.userId = 0;
        dto.cur.colour = PlayerColour.BLUE;
        dto.cur.type = PlayerType.HUMAN;
        dto.availableMoves = [new Move()];
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.availableMoves = [];
        spyOn(service, "makeMoveOnBoard").and.returnValue(new Array<Array<BoardGame>>());
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = [new Player()];
        service.playerRatings = [[], []];
        service.playerHighOptions = [[], []];
        service.playerLowOptions = [[], []];
        service.makeMove(null);
        expect(mockApi.post).toHaveBeenCalled();
    });

    it("Will call the api with the correct endpoint", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const recieved = new BoardGameDTO();
        recieved.availableMoves = [new Move()];
        recieved.cur = new Player();
        recieved.cur.colour = PlayerColour.BLUE;
        recieved.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(recieved));
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(new Array<Array<BoardGame>>());
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        spyOn(service, "rateMove");
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.cur = new Player();
        dto.lastMove = null;
        dto.players = [new Player()];
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = dto.players;
        service.playerRatings = [[], []];
        service.playerHighOptions = [[], []];
        service.playerLowOptions = [[], []];
        service.makeMove(null);
        expect(mockApi.post).toHaveBeenCalledWith("Game/makeMove", dto);
    });

    it("Will call the api with a BoardgameDto with the last move set", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const recieved = new BoardGameDTO();
        recieved.availableMoves = [new Move()];
        recieved.cur = new Player();
        recieved.cur.colour = PlayerColour.BLUE;
        recieved.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(recieved));
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(new Array<Array<BoardGame>>());
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        spyOn(service, "rateMove");
        const move = new Move();
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = [new Player()];
        service.playerRatings = [[], []];
        service.playerHighOptions = [[], []];
        service.playerLowOptions = [[], []];
        service.makeMove(move);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.cur = new Player();
        dto.lastMove = move;
        dto.players = [new Player()];
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = dto.players;
        service.playerRatings = [[], []];
        service.playerHighOptions = [[], []];
        service.playerLowOptions = [[], []];
        service.makeMove(null);
        expect(mockApi.post).toHaveBeenCalledWith("Game/makeMove", dto);
    });


    it("Will call the api to get a new board", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.availableMoves = [new Move()];
        dto.players = [new Player()];
        dto.cur = new Player();
        dto.cur.type = PlayerType.HUMAN;
        dto.cur.colour = 100;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.curPlayer = new Player();
        service.createGame(null, [new Player()]);
        expect(mockApi.post).toHaveBeenCalled();
    });

    it("Will call the api with the correct size arguement", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const recieved = new BoardGameDTO();
        recieved.availableMoves = [new Move()];
        recieved.players = [new Player()];
        recieved.cur = new Player();
        recieved.cur.colour = 100;
        recieved.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(recieved));
        service = new GameService(mockApi);
        const dto = new BoardCreationDTO();
        dto.size = 2;
        dto.players = [new Player(), new Player()];
        service.players = [new Player()];
        service.createGame(2, dto.players);
        expect(mockApi.post).toHaveBeenCalledWith("Game/createBoard", dto);
    });

    it("Will call the api with the correct players arguement", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const recieved = new BoardGameDTO();
        recieved.availableMoves = [new Move()];
        recieved.players = [new Player()];
        recieved.cur = new Player();
        recieved.cur.colour = 100;
        recieved.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(recieved));
        service = new GameService(mockApi);
        const dto = new BoardCreationDTO();
        const players = [
            new Player()
        ];
        dto.size = null;
        dto.players = players;
        service.createGame(null, players);
        expect(mockApi.post).toHaveBeenCalledWith("Game/createBoard", dto);
    });

    it("Will set the current player to the cur player on the boardDto", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const player = new Player();
        const dto = new BoardGameDTO();
        dto.cur = player;
        dto.availableMoves = [new Move()];
        dto.players = [new Player()];
        dto.cur.colour = 100;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.createGame(null, dto.players);
        expect(service.getCurrentPlayer()).toBe(player);
    });

    it("Will set the players list to that returned by the api", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const player = new Player();
        player.colour = 1;
        const players = [new Player(), new Player()];
        const dto = new BoardGameDTO();
        dto.cur = player;
        dto.players = players;
        dto.availableMoves = [new Move()];
        dto.cur.colour = 100;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.createGame(null, dto.players);
        expect(service.players).toBe(players);
    });

    it("Will return the player whos colour doesn't match the current player", () => {
        service.curPlayer = new Player();
        service.curPlayer.colour = 10;
        const other = new Player();
        other.colour = 1000;
        service.players = [service.curPlayer, other];
        const result = service.getNextPlayer();
        expect(result).toBe(other);
    });

    it("Will set the board returned on board creation as it's board", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.availableMoves = [new Move()];
        dto.cur = new Player();
        dto.cur.type = PlayerType.HUMAN;
        dto.cur.colour = 100;
        dto.players = [new Player()];
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.playerRatings = [[]];
        service.playerLowOptions = [[]];
        service.playerHighOptions = [[]];
        service.createGame(null, dto.players);
        expect(service.board).toBe(dto.game);
    });

    it("Will set the board returned after a move as it's board", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.availableMoves = [new Move()];
        dto.cur = new Player();
        dto.cur.colour = 100;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        service.players = [new Player()];
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.curPlayer.colour = 100;
        service.playerHighOptions = [[]];
        service.playerLowOptions = [[]];
        service.playerRatings = [[]];
        spyOn(service, "makeMoveOnBoard").and.returnValue(new Array<Array<BoardGame>>());
        service.makeMove(null);
        expect(service.board).toBe(dto.game);

    });

    it("Will emit an event when the board has been updated", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.availableMoves = [new Move()];
        dto.cur = new Player();
        dto.cur.colour = 10;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        spyOn(service.boardUpdatedEvent, "emit");
        spyOn(service, "makeMoveOnBoard").and.returnValue(null);
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.curPlayer.colour = 100;
        service.players = [new Player()];
        service.playerRatings = [[]];
        service.playerHighOptions = [[]];
        service.playerLowOptions = [[]];
        service.makeMove(null);
        expect(service.boardUpdatedEvent.emit).toHaveBeenCalledWith(dto.game);
    });

    it("Will emit an event when the board has been created", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.availableMoves = [new Move()];
        dto.players = [new Player()];
        dto.cur = new Player();
        dto.cur.colour = PlayerColour.BLUE;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        spyOn(service.boardUpdatedEvent, "emit");
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        service.players = [new Player()];
        service.createGame(null, dto.players);
        expect(service.boardUpdatedEvent.emit).toHaveBeenCalledWith(dto.game);
    });

    it("Will set the available moves to what the api had", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.game = new Array<Array<BoardGame>>();
        dto.availableMoves = new Array<Move>();
        dto.cur = new Player();
        dto.cur.colour = PlayerColour.BLUE;
        dto.availableMoves = [];
        const ratings = new RatingDTO();
        ratings.latest = 0;
        ratings.highOption = 0;
        ratings.lowOption = 0;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(null);
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        spyOn(service, "rateMove").and.stub();
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = [new Player()];
        service.players[0].colour = PlayerColour.BLUE + 1;
        service.playerRatings = [[]];
        service.playerLowOptions = [[]];
        service.playerHighOptions = [[]];
        service.makeMove(null);
        expect(service.getAvailableMoves()).toBe(dto.availableMoves);
    });

    it("Will register the move made", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        mockApi.post.and.returnValue(Observable.of());
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(null);
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        const move = new Move();
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.makeMove(move);
        expect(service.lastMove).toBe(move);
    });

    it("Will assign the last move from the dto as the last move", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        const dto = new BoardGameDTO();
        dto.lastMove = new Move();
        dto.availableMoves = [new Move()];
        dto.cur = new Player();
        dto.cur.colour = PlayerColour.BLUE;
        dto.cur.type = PlayerType.HUMAN;
        mockApi.post.and.returnValue(Observable.of(dto));
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(null);
        spyOn(service, "getNextPlayer").and.returnValue(new Player());
        spyOn(service, "rateMove").and.stub();
        service.curPlayer = new Player();
        service.curPlayer.type = PlayerType.HUMAN;
        service.players = [new Player()];
        service.playerRatings = [[]];
        service.playerLowOptions = [[]];
        service.playerHighOptions = [[]];
        service.makeMove(null);
        expect(service.lastMove).toBe(dto.lastMove);
    });

    it("Will not reasign the last move if the dto does not have one", () => {
        const mockApi = jasmine.createSpyObj("ApiService", ["post"]);
        mockApi.post.and.returnValue(Observable.of());
        service = new GameService(mockApi);
        spyOn(service, "makeMoveOnBoard").and.returnValue(null);
        spyOn(service, "getNextPlayer").and.returnValue(null);
        service.curPlayer = new Player();
        service.makeMove(new Move());
        expect(service.lastMove).not.toBeNull();
    });
});
