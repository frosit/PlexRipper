<template>
	<q-toolbar class="media-overview-bar">
		<!--	Title	-->
		<q-toolbar-title style="overflow: visible">
			<QRow
				align="center"
				justify="start">
				<Transition
					appear
					enter-active-class="animated fadeInLeft"
					leave-active-class="animated fadeOutLeft">
					<QCol
						v-if="detailMode"
						cols="auto">
						<q-btn
							flat
							icon="mdi-arrow-left"
							size="xl"
							@click="$emit('action', 'back')" />
					</QCol>
				</Transition>
				<QCol cols="auto">
					<q-list class="no-background">
						<!-- All Media Mode Header -->
						<q-item
							v-if="libraryId === 0"
							class="q-pa-none">
							<q-item-section avatar>
								<QFab
									label-position="left"
									square
									flat
									push
									vertical-actions-align="left"
									icon="mdi-keyboard-arrow-down"
									direction="down">
									<template #label>
										<QRow
											justify="center"
											align="center">
											<QCol
												cols="auto"
												class="q-mr-md">
												<QMediaTypeIcon
													:media-type="mediaType"
													:size="36" />
											</QCol>
											<QCol>
												<QText size="h5">
													{{ headerText }}
												</QText>
											</QCol>
										</QRow>
									</template>

									<QFabAction
										v-for="(type, i) in [PlexMediaType.Movie, PlexMediaType.TvShow].filter(x => x !== mediaType)"
										:key="i"
										square
										outline
										:label-position="'right'"
										class="blur"
										@click="mediaOverviewStore.changeAllMediaOverviewType(type)">
										<template #icon>
											<QMediaTypeIcon
												:media-type="type"
												:size="36"
												class="q-mr-md" />
										</template>
										<template #label>
											<QText
												size="h5"
												:value="mediaTypeToAllText(type)" />
										</template>
									</QFabAction>
								</QFab>
							</q-item-section>
							<q-item-section />
						</q-item>
						<!-- Single Library Mode -->
						<q-item v-else>
							<q-item-section avatar>
								<QMediaTypeIcon
									:media-type="library?.type"
									:size="36"
									class="mx-3" />
							</q-item-section>
							<q-item-section>
								<q-item-label>
									{{ server ? serverStore.getServerName(server.id) : $t('general.commands.unknown') }}
									{{ $t('general.delimiter.dash') }}
									{{ library ? libraryStore.getLibraryName(library.id) : $t('general.commands.unknown') }}
								</q-item-label>
								<q-item-label
									v-if="library && !detailMode"
									caption>
									{{ libraryCountFormatted }}
									{{ $t('general.delimiter.dash') }}
									<QFileSize :size="library.mediaSize" />
								</q-item-label>
							</q-item-section>
						</q-item>
					</q-list>
				</QCol>
				<QCol align-self="center">
					<q-input
						v-model="mediaOverviewStore.filterQuery"
						:debounce="300"
						outlined
						input-style="font-size: 1.25rem"
						rounded>
						<template #prepend>
							<q-icon
								name="mdi-magnify"
								class="q-ml-sm" />
						</template>
						<template #append>
							<q-icon
								v-if="mediaOverviewStore.filterQuery !== ''"
								name="mdi-close"
								class="cursor-pointer q-mr-sm"
								@click="mediaOverviewStore.filterQuery = ''" />
						</template>
					</q-input>
				</QCol>
			</QRow>
		</q-toolbar-title>

		<!--	Download button	-->
		<VerticalButton
			v-if="mediaOverviewStore.showDownloadButton"
			:height="barHeight"
			:label="$t('general.commands.download')"
			:width="verticalButtonWidth"
			icon="mdi-download"
			cy="media-overview-bar-download-button"
			@click="downloadCommandBus.emit('download')" />

		<!--	Selection Dialog Button	-->
		<VerticalButton
			v-if="mediaOverviewStore.showSelectionButton"
			:height="barHeight"
			:label="$t('general.commands.selection')"
			:width="verticalButtonWidth"
			icon="mdi-select-marker"
			@click="$emit('action', 'selection-dialog')" />

		<!--	Refresh library button	-->
		<VerticalButton
			v-if="!mediaOverviewStore.allMediaMode && !detailMode"
			:height="barHeight"
			:label="$t('general.commands.refresh')"
			:width="verticalButtonWidth"
			cy="media-overview-refresh-library-btn"
			icon="mdi-refresh"
			@click="$emit('action', 'refresh-library');" />

		<!--	Media Options button	-->
		<VerticalButton
			v-if="mediaOverviewStore.allMediaMode && !detailMode"
			:height="barHeight"
			:label="$t('general.commands.media-options')"
			:width="verticalButtonWidth"
			cy="media-overview-options-btn"
			icon="mdi-tune"
			@click="$emit('action', 'media-options-dialog');" />

		<!--	View mode	-->
		<VerticalButton
			v-if="!detailMode"
			:height="barHeight"
			:label="$t('general.commands.view')"
			:width="verticalButtonWidth"
			cy="change-view-mode-btn"
			icon="mdi-eye">
			<q-menu
				anchor="bottom left"
				auto-close
				self="top left">
				<q-list>
					<q-item
						v-for="(viewOption, i) in viewOptions"
						:key="i"
						:data-cy="`view-mode-${viewOption.viewMode.toLowerCase()}-btn`"
						clickable
						style="min-width: 200px"
						@click="changeView(viewOption.viewMode)">
						<!-- View mode options -->
						<q-item-section avatar>
							<q-icon
								v-if="isSelected(viewOption.viewMode)"
								name="mdi-check" />
						</q-item-section>
						<!--	Is selected icon	-->
						<q-item-section> {{ viewOption.label }}</q-item-section>
					</q-item>
				</q-list>
			</q-menu>
		</VerticalButton>
	</q-toolbar>
</template>

<script lang="ts" setup>
import { get } from '@vueuse/core';
import { PlexMediaType, ViewMode } from '@dto';
import type { IMediaOverviewBarActions, IViewOptions } from '@interfaces';
import {
	useLibraryStore,
	useMediaOverviewBarDownloadCommandBus,
	useMediaOverviewStore,
	useServerStore,
	useI18n, useSettingsStore,
} from '#imports';

const libraryStore = useLibraryStore();
const serverStore = useServerStore();
const mediaOverviewStore = useMediaOverviewStore();
const downloadCommandBus = useMediaOverviewBarDownloadCommandBus();

const { t } = useI18n();
const settingsStore = useSettingsStore();

const props = withDefaults(defineProps<{
	mediaType: PlexMediaType;
	libraryId: number;
	detailMode?: boolean;
}>(), {
	libraryId: 0,
	detailMode: false,
});

defineEmits<{
	(e: 'action', payload: IMediaOverviewBarActions): void;
}>();

const barHeight = ref(85);
const verticalButtonWidth = ref(120);

const library = computed(() => libraryStore.getLibrary(props.libraryId));
const server = computed(() => serverStore.getServer(get(library)?.plexServerId ?? -1));

function isSelected(viewMode: ViewMode) {
	return mediaOverviewStore.getMediaViewMode === viewMode;
}

const libraryCountFormatted = computed(() => {
	const libraryValue = get(library);
	if (libraryValue) {
		switch (props.mediaType) {
			case PlexMediaType.Movie:
				return `${libraryValue.count} Movies`;
			case PlexMediaType.TvShow:
				return `${libraryValue.count} TvShows - ${libraryValue.seasonCount} Seasons - ${libraryValue.episodeCount} Episodes`;
			default:
				return `Library type ${props.mediaType} is not supported in the media count`;
		}
	}
	return 'unknown media count';
});

const viewOptions = computed((): IViewOptions[] => {
	return [
		{
			label: 'Poster View',
			viewMode: ViewMode.Poster,
		},
		{
			label: 'Table View',
			viewMode: ViewMode.Table,
		},
	];
});

const headerText = computed(() => {
	switch (props.mediaType) {
		case PlexMediaType.Movie:
		case PlexMediaType.TvShow:
			return mediaTypeToAllText(props.mediaType);
		default:
			return `Library type ${props.mediaType} is not supported`;
	}
});

function changeView(viewMode: ViewMode) {
	mediaOverviewStore.clearSort();
	settingsStore.updateDisplayMode(props.mediaType, viewMode);
}

function mediaTypeToAllText(mediaType: PlexMediaType): string {
	switch (mediaType) {
		case PlexMediaType.Movie:
			return t('components.media-overview-bar.all-media-mode.movies');
		case PlexMediaType.TvShow:
			return t('components.media-overview-bar.all-media-mode.tv-shows');
		default:
			return t('general.error.unknown');
	}
}
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.media-overview-bar {
  @extend .default-border;
  min-height: $media-overview-bar-height;
}

.q-fab__label {
  max-height: none;
}
</style>
