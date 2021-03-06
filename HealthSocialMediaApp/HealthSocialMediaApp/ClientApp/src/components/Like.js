import React from "react";
import { IconButton } from "@material-ui/core";
import FavoriteIcon from "@material-ui/icons/Favorite";
import { grey } from "@material-ui/core/colors";

const Like = ({ isLiked, onToggle }) => {
	return (
		<IconButton aria-label="like" onClick={onToggle}>
			<FavoriteIcon style={{ color: isLiked ? "#44b98a" : grey[500] }} />
		</IconButton>
	);
};

export { Like };
