<template>
	<div
		class="media-table"
		data-cy="media-table">
		<MediaTableHeader
			:columns="mediaTableColumns"
			selectable
			:selected="mediaOverviewStore.isRootSelected"
			class="media-table--header"
			@selected="mediaOverviewStore.setRootSelected($event)" />
		<div
			id="media-table-scroll"
			ref="qTableRef"
			:class="['media-table--content', isScrollable ? 'scroll' : '']"
			data-cy="media-table-scroll">
			<template v-if="disableIntersection">
				<MediaTableRow
					v-for="(row, index) in rows"
					:key="index"
					:index="index"
					:data-cy="`media-table-row-${index}`"
					:columns="mediaTableColumns"
					:row="row"
					selectable
					:selected="isSelected(row.id)"
					:disable-highlight="disableHighlight"
					:disable-hover-click="disableHoverClick"
					@selected="updateSelectedRow(row.id, $event)" />
			</template>
			<template v-else>
				<q-intersection
					v-for="(row, index) in rows"
					:key="row.id"
					:once="disableIntersection"
					class="media-table--intersection highlight-border-box"
					:data-scroll-index="index">
					<MediaTableRow
						:index="index"
						:data-cy="`media-table-row-${index}`"
						:columns="mediaTableColumns"
						:row="row"
						selectable
						:selected="isSelected(row.id)"
						:disable-highlight="disableHighlight"
						:disable-hover-click="disableHoverClick"
						@selected="updateSelectedRow(row.id, $event)" />
				</q-intersection>
			</template>
		</div>
	</div>
</template>

<script setup lang="ts">
import Log from 'consola';
import { get, set, useScroll } from '@vueuse/core';
import type { PlexMediaSlimDTO } from '@dto';
import type { ISelection } from '@interfaces';
import {
	triggerBoxHighlight,
	listenMediaOverviewScrollToCommand,
	useMediaOverviewStore,
} from '#imports';
import { getMediaTableColumns } from '~/composables/mediaTableColumns';

const mediaOverviewStore = useMediaOverviewStore();
const mediaTableColumns = getMediaTableColumns();
const qTableRef = ref<HTMLElement | null>(null);
const scrollTargetElement = ref<HTMLElement | null>(null);
const autoScrollEnabled = ref(false);

withDefaults(
	defineProps<{
		rows: Readonly<PlexMediaSlimDTO[]>;
		disableHoverClick?: boolean;
		disableHighlight?: boolean;
		disableIntersection?: boolean;
		isScrollable?: boolean;
	}>(),
	{
		disableHoverClick: false,
		disableHighlight: false,
		disableIntersection: false,
		isScrollable: true,
	},
);

defineEmits<{
	(e: 'row-click', payload: PlexMediaSlimDTO): void;
}>();

function isSelected(mediaId: number) {
	return (mediaOverviewStore.selection?.keys ?? []).includes(mediaId);
}

function updateSelectedRow(mediaId: number, state: boolean) {
	mediaOverviewStore.setSelection({
		...mediaOverviewStore.selection,
		keys: state
			? [...(mediaOverviewStore.selection?.keys ?? []), mediaId]
			: (mediaOverviewStore.selection?.keys ?? []).filter((x) => x !== mediaId),
		allSelected: false,
	} as ISelection);
}

function scrollToIndex(index: number) {
	// noinspection TypeScriptValidateTypes
	const element: HTMLElement | null = get(qTableRef)?.querySelector(`[data-scroll-index="${index}"]`) ?? null;
	if (!element) {
		Log.error(`Could not find scroll target element`, `[data-scroll-index="${index}"]`);
		return;
	}

	set(scrollTargetElement, element);
	set(autoScrollEnabled, true);

	const elementRect = get(scrollTargetElement)?.getBoundingClientRect();
	// Scroll if not visible
	if ((elementRect?.bottom ?? 0) >= 0 && (elementRect?.top ?? 0) <= window.innerHeight) {
		triggerBoxHighlight(element);
	} else {
		get(scrollTargetElement)?.scrollIntoView({
			block: 'start',
			behavior: 'smooth',
		});
	}
}

onMounted(() => {
	const lastMediaItemViewed = get(mediaOverviewStore.lastMediaItemViewed);
	if (lastMediaItemViewed && lastMediaItemViewed.sortIndex > 0) {
		// If we have a last viewed media item, scroll to it
		scrollToIndex(lastMediaItemViewed.sortIndex - 1);
	}

	// Listen for scroll to letter command
	listenMediaOverviewScrollToCommand((letter) => {
		if (!get(qTableRef)) {
			Log.error('qTableRef is null');
			return;
		}

		// We have to revert to normal title sort otherwise the index will be wrong
		mediaOverviewStore.clearSort();
		const index = mediaOverviewStore.scrollDict[letter] ? mediaOverviewStore.scrollDict[letter] : 0;

		scrollToIndex(index);
	});
	// Setup stopped scrolling event listener
	useScroll(get(qTableRef), {
		onStop() {
			// Don't highlight if the user scrolls manually
			if (!get(autoScrollEnabled)) {
				return;
			}
			set(autoScrollEnabled, false);
			triggerBoxHighlight(get(scrollTargetElement));
		},
	});
});
</script>

<style lang="scss">
@import '@/assets/scss/variables.scss';

.media-table {
  &--header,
  &--intersection,
  &--intersection > div {
    height: $media-table-row-height;
  }

  &--content {
    max-height: calc($page-height-minus-app-bar - $media-overview-bar-height - $media-table-row-height);
  }
}

.row-title {
  font-weight: bold;
  min-width: 300px;
  max-width: 300px;

  &--hover {
    cursor: pointer;

    :hover {
      color: $primary;
    }
  }
}
</style>
