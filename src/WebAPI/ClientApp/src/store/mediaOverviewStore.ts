import { isEqual, orderBy } from 'lodash-es';
import { defineStore, acceptHMRUpdate } from 'pinia';
import { get } from '@vueuse/core';
import { PlexMediaType, ViewMode, type PlexMediaSlimDTO } from '@dto';
import type { IMediaOverviewSort } from '@composables/event-bus';
import type { ISelection } from '@interfaces';
import { plexLibraryApi, plexMediaApi } from '@api';
import { map, tap } from 'rxjs/operators';
import { iif, defer, type Observable, of } from 'rxjs';
import { useSettingsStore, useLibraryStore } from '#imports';

export const useMediaOverviewStore = defineStore('MediaOverviewStore', () => {
	const state = reactive<{
		libraryId: number;
		items: Readonly<PlexMediaSlimDTO[]>;
		sortedItems: Readonly<PlexMediaSlimDTO[]>;
		itemsLength: number;
		sortedState: IMediaOverviewSort[];
		scrollDict: Record<string, number>;
		scrollAlphabet: string[];
		selection: ISelection;
		downloadButtonVisible: boolean;
		mediaType: PlexMediaType;
		filterQuery: string;
		lastMediaItemViewed: PlexMediaSlimDTO | null;
		loading: boolean;
	}>({
		libraryId: 0,
		items: [],
		sortedItems: [],
		itemsLength: 0,
		sortedState: [],
		scrollDict: { '#': 0 },
		scrollAlphabet: [],
		selection: { keys: [], allSelected: false, indexKey: 0 },
		downloadButtonVisible: false,
		mediaType: PlexMediaType.None,
		filterQuery: '',
		lastMediaItemViewed: null,
		loading: false,
	});

	const settingsStore = useSettingsStore();
	const libraryStore = useLibraryStore();

	const actions = {
		requestMedia({
			mediaType,
			page = 0,
			size = 0,
		}: {
			mediaType: PlexMediaType;
			page: number;
			size: number;
		}): Observable<PlexMediaSlimDTO[]> {
			if (state.loading) {
				return of([]);
			}
			state.loading = true;
			return iif(
				() => state.libraryId === 0,
				defer(() =>
					plexMediaApi.getAllMediaByTypeEndpoint({
						mediaType: mediaType,
						page,
						size,
						filterOwnedMedia: settingsStore.generalSettings.hideMediaFromOwnedServers,
						filterOfflineMedia: settingsStore.generalSettings.hideMediaFromOfflineServers,
					}),
				),
				defer(() =>
					plexLibraryApi.getPlexLibraryMediaEndpoint(state.libraryId, {
						page,
						size,
					}),
				),
			).pipe(
				map((response) => {
					if (response && response.isSuccess) {
						return response.value ?? [];
					}
					return [];
				}),
				tap((data) => {
					actions.setMedia(data, mediaType);
					state.loading = false;
				}),
			);
		},
		setMedia(items: PlexMediaSlimDTO[], mediaType: PlexMediaType) {
			state.items = Object.freeze(items);
			state.itemsLength = state.items.length;
			state.mediaType = mediaType;
			state.filterQuery = '';
		},
		changeAllMediaOverviewType(mediaType: PlexMediaType) {
			state.mediaType = mediaType;
			settingsStore.displaySettings.allOverviewViewMode = mediaType;
			actions.requestMedia({ mediaType, page: 0, size: 0 }).subscribe();
		},
		setFirstLetterIndex() {
			// Create scroll indexes for each letter
			state.scrollDict = {};
			state.scrollDict['#'] = 0;
			// Check for occurrence of title with alphabetic character
			const sortTitles = get(getters.getMediaItems).map((x) => x.title[0]?.toLowerCase() ?? '#');
			let lastIndex = 0;
			const alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.toLowerCase();

			for (const letter of alphabet) {
				lastIndex = sortTitles.findIndex((x, idx) => idx >= lastIndex && x === letter);
				if (lastIndex > -1) {
					state.scrollDict[letter] = lastIndex;
				}
			}
			state.scrollAlphabet = Object.keys(state.scrollDict);
		},
		setSelection(selection: ISelection) {
			state.selection = selection;
		},
		getMediaIndex(mediaId: number): number {
			return state.items.findIndex((x) => x.id === mediaId);
		},
		setSelectionRange(min: number, max: number) {
			actions.setSelection({
				indexKey: state.selection.indexKey,
				keys: get(getters.getMediaItems)
					.filter((x) => x.sortIndex >= min && x.sortIndex <= max)
					.map((x) => x.id),
				allSelected: false,
			} as ISelection);
		},
		setRootSelected(value: boolean) {
			actions.setSelection({
				indexKey: state.selection?.indexKey ?? 0,
				keys: value ? state.items.map((x) => x.id) : [],
				allSelected: value,
			} as ISelection);
		},
		clearSort() {
			state.sortedState = [];
			state.sortedItems = [];
		},
		sortMedia(event: IMediaOverviewSort) {
			const newSortedState = [...state.sortedState];
			const index = newSortedState.findIndex((x) => x.field === event.field);
			if (index > -1) {
				newSortedState.splice(index, 1);
			}
			if (event.sort) {
				newSortedState.unshift(event);
			}

			// Prevent unnecessary sorting
			if (isEqual(state.sortedState, newSortedState)) {
				return;
			}
			const lodashFormat = newSortedState.map((x) => {
				return {
					field: x.field,
					sort: x.sort !== 'no-sort' ? x.sort : false,
				};
			});
			state.sortedItems = Object.freeze(
				orderBy(
					state.items, // Items to sort
					lodashFormat.map((x) => x.field), // Sort by field
					lodashFormat.map((x) => x.sort), // Sort by sort, asc or desc
				),
			);
			state.sortedState = newSortedState;
		},
	};

	const getters = {
		hasSelectedMedia: computed((): boolean => {
			return state.selection.keys.length > 0;
		}),
		hasNoSearchResults: computed((): boolean => {
			return state.filterQuery != '' && get(getters.getMediaItems).length === 0;
		}),
		allMediaMode: computed(() => state.libraryId === 0),
		library: computed(() => libraryStore.getLibrary(state.libraryId)),
		getMediaItems: computed((): Readonly<PlexMediaSlimDTO[]> => {
			// Currently sorting
			const query = state.filterQuery.toLowerCase();
			if (state.sortedState.length > 0) {
				if (state.filterQuery != '') {
					return state.sortedItems.filter((x) => x.searchTitle.includes(query));
				}
				return state.sortedItems;
			} else {
				if (state.filterQuery != '') {
					return state.items.filter((x) => x.searchTitle.includes(query));
				}
				return state.items;
			}
		}),
		getMediaViewMode: computed((): ViewMode => {
			switch (state.mediaType) {
				case PlexMediaType.Movie:
					return settingsStore.displaySettings.movieViewMode;
				case PlexMediaType.TvShow:
					return settingsStore.displaySettings.tvShowViewMode;
				default:
					return ViewMode.Poster;
			}
		}),
		showSelectionButton: computed((): boolean => {
			return get(getters.getMediaViewMode) === ViewMode.Table;
		}),
		showDownloadButton: computed((): boolean => {
			return state.downloadButtonVisible || (getters.hasSelectedMedia && get(getters.getMediaViewMode) === ViewMode.Table);
		}),
		isRootSelected: computed((): boolean | null => {
			if (state.selection?.keys.length === state.itemsLength) {
				return true;
			}

			if (state.selection?.keys.length === 0) {
				return false;
			}

			return null;
		}),
	};

	watch(getters.getMediaItems, () => actions.setFirstLetterIndex());

	return {
		...toRefs(state),
		...actions,
		...getters,
	};
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useMediaOverviewStore, import.meta.hot));
}
