import {IObservable} from "@/lib/observer/IObservable.ts";

export class EventEmitter<T> extends IObservable<T> {
    emit(data: T): void {
        this._state = data;
        this.listeners.forEach(listener => listener(data));
    }
}