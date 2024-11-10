import { acceptHMRUpdate, defineStore } from 'pinia';
import { type Observable, of, Subject } from 'rxjs';
import type { ISetupResult } from '@interfaces';
import { DialogType } from '@enums';

interface DialogState {
	name: string;
	state: boolean;
	data: unknown;
}

export const useDialogStore = defineStore('DialogStore', () => {
	const state = reactive<{ dialogUpdate: Subject<DialogState> }>({
		dialogUpdate: new Subject<DialogState>(),
	});
	const actions = {
		setup(): Observable<ISetupResult> {
			return of({ name: useAlertStore.name, isSuccess: true });
		},
		setCheckServerConnectionsDialogState(visible: boolean, data: unknown): void {
			state.dialogUpdate.next({ name: DialogType.CheckServerConnectionDialogName, state: visible, data });
		},
	};
	const getters = {
		getDialogState: (): Observable<DialogState> => {
			return state.dialogUpdate.asObservable();
		},
	};
	return {
		...toRefs(state),
		...actions,
		...getters,
	};
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useDialogStore, import.meta.hot));
}
