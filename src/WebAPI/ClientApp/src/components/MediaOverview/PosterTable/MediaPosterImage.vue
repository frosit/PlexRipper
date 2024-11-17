<template>
	<q-card
		square
		flat
		:class="{ 'media-poster--fallback': fallback }"
		style="height: 100%">
		<q-card-section
			v-if="fallback">
			<QRow justify="center">
				<QCol cols="auto">
					<QMediaTypeIcon
						:size="60"
						:media-type="mediaType" />
				</QCol>
			</QRow>
		</q-card-section>

		<q-card-section :class="{ 'q-py-none': fallback }">
			<QText
				:value="mediaItem.title"
				bold="bold"
				align="center"
				size="subtitle1"
				class="media-poster--title" />
			<QText
				v-if="allMediaMode"
				align="center"
				size="subtitle2"
				:value="serverStore.getServerName(mediaItem.plexServerId)" />
		</q-card-section>

		<QCardActions
			align="center"
			class="media-poster--actions">
			<QRow :justify="mediaType === PlexMediaType.TvShow ? 'between' : 'center'">
				<QCol cols="auto">
					<BaseButton
						icon="mdi-download"
						size="xl"
						flat
						:outline="false"
						@click="$emit('download')" />
				</QCol>
				<QCol cols="auto">
					<BaseButton
						v-if="mediaType === PlexMediaType.TvShow"
						icon="mdi-magnify"
						:outline="false"
						size="xl"
						flat
						@click="$emit('open-media-details', mediaItem)" />
				</QCol>
			</QRow>
		</QCardActions>
	</q-card>
</template>

<script setup lang="ts">
import { type PlexMediaSlimDTO, PlexMediaType } from '@dto';
import { useServerStore } from '@store';

const serverStore = useServerStore();

const props = withDefaults(defineProps<{
	mediaItem: PlexMediaSlimDTO;
	fallback?: boolean;
	allMediaMode?: boolean;
}>(), {
	fallback: false,
	allMediaMode: false,
});

defineEmits<{
	(e: 'download'): void;
	(e: 'open-media-details', payload: PlexMediaSlimDTO): void;
}>();

const mediaType = computed(() => props.mediaItem?.type ?? PlexMediaType.Unknown);
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.media-poster {
  &--fallback {
    background-color: transparent !important;
    padding: 0 !important;
  }

  &--actions {
    position: absolute;
    text-align: center;
    bottom: 0;
    left: 0;
    right: 0;
  }
}
</style>
