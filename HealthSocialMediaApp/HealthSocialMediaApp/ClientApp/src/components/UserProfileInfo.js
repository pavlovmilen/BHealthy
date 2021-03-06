import React from "react";
import { Link as RouterLink } from "react-router-dom";

import { Typography, Grid, Avatar, Button, Box, Link } from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
import { blue } from "@material-ui/core/colors";

import { useCurrentUserId } from "../hooks/useCurrentUserId";

const useStyles = makeStyles(theme => ({
	media: {
		width: "100%",
		maxWidth: "150px",
		height: "150px",
		[theme.breakpoints.down("sm")]: {
			margin: "0 auto 20px auto"
		},
		backgroundColor: blue[500]
	},
	profileInfo: {
		marginTop: "8px",
		[theme.breakpoints.down("sm")]: {
			justifyContent: "space-evenly"
		}
	},
	profileInfoItem: {
		marginRight: "1rem",
		[theme.breakpoints.down("sm")]: {
			marginRight: "0px"
		}
	},
	siteLinks: {
		textDecorationLine: "none !important"
	}
}));

const UserProfileInfo = ({
	user,
	onUnfollow,
	amountOfPosts,
	onFollow,
	isOwnProfile,
	isFollowingThisUser
}) => {
	const classes = useStyles();

	const currentUserId = useCurrentUserId();

	return (
		<>
			<Grid item xs={12} sm={12} md={4} lg={4} xl={4}>
				<Avatar className={classes.media}>
					<Typography variant="h2">
						{user.userName[0] + user.userName[1]}
					</Typography>
				</Avatar>
			</Grid>
			<Grid item xs={12} sm={12} md={8} lg={8} xl={8}>
				<Grid item xs={12} sm={12} md={12} lg={12} xl={12}>
					<Typography variant="h4">{user.userName}</Typography>
				</Grid>
				<Grid item xs={12} sm={12} md={12} lg={12} xl={12}>
					<Box display="flex" className={classes.profileInfo}>
						<Box className={classes.profileInfoItem}>
							<Typography variant="h6">Posts</Typography>
							<Typography>{amountOfPosts}</Typography>
						</Box>
						<Link
							component={RouterLink}
							to={`/follows/${user.id}`}
							key={user.id}
							className={classes.siteLinks}
							color="inherit"
						>
							<Box className={classes.profileInfoItem}>
								<Typography>
									<Typography variant="h6">
										Follows
									</Typography>
								</Typography>
								<Typography>{user.followees.length}</Typography>
							</Box>
						</Link>
						<Link
							component={RouterLink}
							to={`/followers/${user.id}`}
							key={user.id}
							className={classes.siteLinks}
							color="inherit"
						>
							<Box className={classes.profileInfoItem}>
								<Box className={classes.profileInfoItem}>
									<Typography>
										<Typography variant="h6">
											Followers
										</Typography>
									</Typography>
								</Box>
								<Typography>{user.followers.length}</Typography>
							</Box>
						</Link>
					</Box>
				</Grid>
				<br />
				<Grid item xs={12} sm={12} md={12} lg={12} xl={12}>
					{currentUserId !== null &&
						!isOwnProfile &&
						isFollowingThisUser && (
							<Button
								variant="contained"
								color="secondary"
								onClick={onUnfollow}
							>
								Unfollow
							</Button>
						)}
					{currentUserId !== null &&
						!isOwnProfile &&
						!isFollowingThisUser && (
							<Button
								variant="contained"
								color="primary"
								style={{ color: "#FFFFFF" }}
								onClick={onFollow}
							>
								Follow
							</Button>
						)}
				</Grid>
				<br />
				<Grid item xs={12} sm={12} md={12} lg={12} xl={12}>
					<Typography variant="body1">{user.description}</Typography>
				</Grid>
			</Grid>
		</>
	);
};

export { UserProfileInfo };
